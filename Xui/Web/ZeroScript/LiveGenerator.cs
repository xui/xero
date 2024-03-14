using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

#pragma warning disable RS1035 // Do not use banned APIs for analyzers

namespace Xui.Web.ZeroScript
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

            if (context.Compilation.GetTypeByMetadataName("Xui.Web.ZeroScript.LiveAttribute") is not INamedTypeSymbol attributeSymbol)
                return;

            // if (context.Compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged") is not INamedTypeSymbol notifySymbol)
            //     return;

            var groups = receiver
                .Fields
                .GroupBy<IFieldSymbol, INamedTypeSymbol>(f => f.ContainingType, SymbolEqualityComparer.Default);

            foreach (var group in groups)
                if (ProcessClass(group.Key, group.ToList(), attributeSymbol, context) is string classSource)
                    context.AddSource($"{group.Key.Name}.g.cs", SourceText.From(classSource, Encoding.UTF8));
        }

        private string? ProcessClass(
            INamedTypeSymbol classSymbol,
            List<IFieldSymbol> fields,
            ISymbol attributeSymbol,
            GeneratorExecutionContext context)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                // TODO: Support classes/structs that are not top level
                return null;
            }

            var members = new StringBuilder();
            members.AppendLine($$"""
                bool isBatching = false;
                bool isBatchChanged = false;
                public Action? OnChanging { get; set; }
                public Action? OnChanged { get; set; }

                public {{classSymbol.Name}}()
                {
                }

                public IDisposable Batch()
                {
                    return new Batcher(this);
                }

                private void OnPropertyChanged()
                {
                    if (isBatching)
                        isBatchChanged = true;
                    else
                        OnChanged?.Invoke();

                }

                class Batcher : IDisposable
                {
                    readonly {{classSymbol.Name}} viewModel;

                    public Batcher({{classSymbol.Name}} viewModel)
                    {
                        this.viewModel = viewModel;
                        this.viewModel.isBatching = true;
                    }

                    public void Dispose()
                    {
                        viewModel.isBatching = false;
                        if (viewModel.isBatchChanged)
                            viewModel.OnPropertyChanged();
                    }
                }
            """
            );
            members.AppendLine();

            foreach (var fieldSymbol in fields)
            {
                if (CreateField(fieldSymbol, attributeSymbol) is string field)
                {
                    members.AppendLine(field);
                    members.AppendLine();
                }
            }

            var namespaceLine = classSymbol.ContainingNamespace.ToDisplayString();
            namespaceLine = (namespaceLine != "<global namespace>")
                ? $"namespace {namespaceLine};"
                : "";

            return $$"""
                #nullable enable

                {{namespaceLine}}

                public partial class {{classSymbol.Name}}
                {
                {{members.ToString()}}
                }
                """;
        }

        private string? CreateField(IFieldSymbol fieldSymbol, ISymbol attributeSymbol)
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
                return null;
            }

            return $$"""
                public {{fieldType}} {{propertyName}}
                {
                    get 
                    {
                        return this.{{fieldName}};
                    }

                    set
                    {
                        if (!EqualityComparer<{{fieldType}}>.Default.Equals(this.{{fieldName}}, value))
                        {
                            this.{{fieldName}} = value;
                            OnPropertyChanged();
                        }
                    }
                }
            """;
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
                        if (fieldSymbol?.GetAttributes().Any(ad => ad.AttributeClass?.ToDisplayString() == "Xui.Web.ZeroScript.LiveAttribute") == true)
                        {
                            Fields.Add(fieldSymbol);
                        }
                    }
                }
            }
        }
    }
}
