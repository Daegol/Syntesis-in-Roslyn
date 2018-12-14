using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace jfkzad2
{
    class Rewriter : CSharpSyntaxRewriter
    {
        private readonly SemanticModel _model;
        public Rewriter(SemanticModel model)
        {
            _model = model;
        }


        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            var type = node.ChildNodes().OfType<BaseListSyntax>().FirstOrDefault();

                var symbol = _model.GetDeclaredSymbol(node);

            if (type == null)
            {
                node = node.AddBaseListTypes(SyntaxFactory.SimpleBaseType
                        (

                            SyntaxFactory.QualifiedName
                            (
                             SyntaxFactory.IdentifierName("System"),
                             SyntaxFactory.IdentifierName("Object").WithTrailingTrivia(SyntaxFactory.EndOfLine(Environment.NewLine))
                            )
                        ));
                var eol = node.ChildTokens().OfType<SyntaxToken>().ElementAt(1);
                var ws = eol.ReplaceTrivia(eol.TrailingTrivia.Last(), SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " "));
                node = node.ReplaceToken(eol, ws);
                var colon = node.BaseList.ChildTokens().OfType<SyntaxToken>().FirstOrDefault();
                var colonws = colon.WithTrailingTrivia(SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " "));
                node = node.ReplaceToken(colon, colonws);
            }else if(symbol.BaseType.Name == "Object" && (symbol.BaseType.ToString() != "Object" && symbol.BaseType.ToString() != "System.Object"))
            {
                var blist = node.BaseList.Types.Insert(0, SyntaxFactory.SimpleBaseType(
                  SyntaxFactory.QualifiedName
                             (
                              SyntaxFactory.IdentifierName("System"),
                              SyntaxFactory.IdentifierName("Object")
                             )).WithTrailingTrivia(SyntaxFactory.Whitespace(" ")));
                node = node.ReplaceNode(node.BaseList, SyntaxFactory.BaseList(blist));
                SyntaxToken comma = node.BaseList.ChildTokens().OfType<SyntaxToken>().FirstOrDefault();
                foreach(SyntaxToken token in node.BaseList.ChildTokens().OfType<SyntaxToken>())
                {
                    if (token.ValueText == ",") comma = token;
                }
                var ncomma = comma.WithTrailingTrivia(SyntaxFactory.Whitespace(" "));
                node = node.ReplaceToken(comma, ncomma);
                var colon = node.BaseList.ChildTokens().OfType<SyntaxToken>().FirstOrDefault();
                var colonws = colon.WithTrailingTrivia(SyntaxFactory.SyntaxTrivia(SyntaxKind.WhitespaceTrivia, " "));
                node = node.ReplaceToken(colon, colonws);


            }
            
            return base.VisitClassDeclaration(node);
        }
    }
}
