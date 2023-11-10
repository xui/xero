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
            if (context.SyntaxContextReceiver is not SyntaxReceiver receiver)
                return;

            if (context.Compilation.GetTypeByMetadataName("Xero.LiveAttribute") is not INamedTypeSymbol attributeSymbol)
                return;

            if (context.Compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged") is not INamedTypeSymbol notifySymbol)
                return;

            var groups = receiver
                .Fields
                .GroupBy<IFieldSymbol, INamedTypeSymbol>(f => f.ContainingType, SymbolEqualityComparer.Default);

            foreach (var group in groups)
                if (ProcessClass(group.Key, group.ToList(), attributeSymbol, notifySymbol, context) is string classSource)
                    context.AddSource($"{group.Key.Name}_live.g.cs", SourceText.From(classSource, Encoding.UTF8));
        }

        private string? ProcessClass(
            INamedTypeSymbol classSymbol,
            List<IFieldSymbol> fields,
            ISymbol attributeSymbol,
            ISymbol notifySymbol, GeneratorExecutionContext context)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                // TODO: Support classes/structs that are not top level
                return null;
            }

            var source = new StringBuilder();

            // If the class doesn't implement INotifyPropertyChanged already, add it
            if (!classSymbol.Interfaces.Contains(notifySymbol, SymbolEqualityComparer.Default))
            {
                source.AppendLine("public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;");
                source.AppendLine();
            }

            // Create properties for each field 
            foreach (IFieldSymbol fieldSymbol in fields)
            {
                ProcessField(source, fieldSymbol, attributeSymbol);
                source.AppendLine();
            }

            var namespaceLine = classSymbol.ContainingNamespace.ToDisplayString();
            namespaceLine = (namespaceLine != "<global namespace>")
                ? $"namespace {namespaceLine};"
                : "";

            return $$"""
                #nullable enable

                {{namespaceLine}}

                public partial class {{classSymbol.Name}} : {{notifySymbol.ToDisplayString()}}
                {
                    {{source.ToString()}}
                }
                """;
        }

        private void ProcessField(StringBuilder source, IFieldSymbol fieldSymbol, ISymbol attributeSymbol)
        {
            var fieldName = fieldSymbol.Name;
            var fieldType = fieldSymbol.Type;
            var overriddenNameOpt = fieldSymbol
                .GetAttributes()
                .Single(ad => ad?.AttributeClass?.Equals(attributeSymbol, SymbolEqualityComparer.Default) == true)
                .NamedArguments.SingleOrDefault(kvp => kvp.Key == "PropertyName")
                .Value;

            var propertyName = ChooseName(fieldName, overriddenNameOpt);
            if (propertyName.Length == 0 || propertyName == fieldName)
            {
                // TODO: issue a diagnostic that we can't process this field
                return;
            }

            source.AppendLine($$"""
                public {{fieldType}} {{propertyName}}
                {
                    get 
                    {
                        return this.{{fieldName}};
                    }

                    set
                    {
                        this.{{fieldName}} = value;
                        this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof({{propertyName}})));
                    }
                }
            """);
        }

        string ChooseName(string fieldName, TypedConstant overriddenNameOpt)
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
