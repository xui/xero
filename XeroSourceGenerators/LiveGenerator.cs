using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

#pragma warning disable RS1035 // Do not use banned APIs for analyzers

namespace Xero
{
    [Generator]
    public class LiveGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // retrieve the populated receiver 
            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
                return;

            // get the added attribute, and INotifyPropertyChanged
            var attributeSymbol = context.Compilation.GetTypeByMetadataName("Xero.LiveAttribute");
            if (attributeSymbol is null)
                return;

            var notifySymbol = context.Compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged");
            if (notifySymbol is null)
                return;

            // group the fields by class, and generate the source
            foreach (IGrouping<INamedTypeSymbol, IFieldSymbol> group in receiver.Fields.GroupBy<IFieldSymbol, INamedTypeSymbol>(f => f.ContainingType, SymbolEqualityComparer.Default))
            {
                var classSource = ProcessClass(group.Key, group.ToList(), attributeSymbol, notifySymbol, context);
                if (classSource is not null)
                    context.AddSource($"{group.Key.Name}_live.g.cs", SourceText.From(classSource, Encoding.UTF8));
            }
        }

        private string? ProcessClass(INamedTypeSymbol classSymbol, List<IFieldSymbol> fields, ISymbol attributeSymbol, ISymbol notifySymbol, GeneratorExecutionContext context)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return null; //TODO: issue a diagnostic that it must be top level
            }

            string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            // begin building the generated source
            StringBuilder source = new StringBuilder();
            if (namespaceName == "<global namespace>")
            {
                source.Append($@"
#nullable enable
public partial class {classSymbol.Name} : {notifySymbol.ToDisplayString()}
{{
");

            }
            else
            {
                source.Append($@"
#nullable enable
namespace {namespaceName}
{{
public partial class {classSymbol.Name} : {notifySymbol.ToDisplayString()}
{{
");
            }

            // if the class doesn't implement INotifyPropertyChanged already, add it
            if (!classSymbol.Interfaces.Contains(notifySymbol, SymbolEqualityComparer.Default))
            {
                source.Append("public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;");
            }

            // create properties for each field 
            foreach (IFieldSymbol fieldSymbol in fields)
            {
                ProcessField(source, fieldSymbol, attributeSymbol);
            }

            if (namespaceName == "<global namespace>")
            {
                source.Append("}");
            }
            else
            {
                source.Append("} }");
            }
            return source.ToString();
        }

        private void ProcessField(StringBuilder source, IFieldSymbol fieldSymbol, ISymbol attributeSymbol)
        {
            // get the name and type of the field
            string fieldName = fieldSymbol.Name;
            ITypeSymbol fieldType = fieldSymbol.Type;

            // get the Live attribute from the field, and any associated data
            var attributeData = fieldSymbol.GetAttributes().Single(ad => ad?.AttributeClass?.Equals(attributeSymbol, SymbolEqualityComparer.Default) ?? false);
            var overriddenNameOpt = attributeData.NamedArguments.SingleOrDefault(kvp => kvp.Key == "PropertyName").Value;

            string propertyName = ChooseName(fieldName, overriddenNameOpt);
            if (propertyName.Length == 0 || propertyName == fieldName)
            {
                //TODO: issue a diagnostic that we can't process this field
                return;
            }

            source.Append($@"
public {fieldType} {propertyName} 
{{
get 
{{
    return this.{fieldName};
}}

set
{{
    this.{fieldName} = value;
    this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof({propertyName})));
}}
}}

");
        }

        private string ChooseName(string fieldName, TypedConstant overriddenNameOpt)
        {
            if (overriddenNameOpt.Value is not null)
            {
                return overriddenNameOpt.Value.ToString();
            }

            fieldName = fieldName.TrimStart('_');
            if (fieldName.Length == 0)
                return string.Empty;

            if (fieldName.Length == 1)
                return fieldName.ToUpper();

            return fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
        }

        class SyntaxReceiver : ISyntaxContextReceiver
        {
            public List<IFieldSymbol> Fields { get; } = new List<IFieldSymbol>();

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                // Any field with at least one attribute is a candidate for property generation
                if (context.Node is FieldDeclarationSyntax fieldDeclarationSyntax && fieldDeclarationSyntax.AttributeLists.Count > 0)
                {
                    foreach (var variable in fieldDeclarationSyntax.Declaration.Variables)
                    {
                        // Get the symbol being declared by the field, and keep it if it's annotated
                        var fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;
                        if (fieldSymbol?.GetAttributes().Any(ad => ad.AttributeClass?.ToDisplayString() == "Xero.LiveAttribute") == true)
                        {
                            Fields.Add(fieldSymbol);
                        }
                    }
                }
            }
        }
    }
}
