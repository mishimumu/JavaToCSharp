﻿using System.Collections.Generic;
using com.github.javaparser.ast.body;
using com.github.javaparser.ast.expr;
using JavaToCSharp.Statements;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JavaToCSharp.Expressions
{
    public class LambdaExpressionVisitor : ExpressionVisitor<LambdaExpr>
    {
        public override ExpressionSyntax Visit(ConversionContext context, LambdaExpr expr)
        {
            var bodyStatement = expr.getBody();
            var bodyStatementSyntax = StatementVisitor.VisitStatement(context, bodyStatement);
            var lambdaExpressionSyntax = SyntaxFactory.ParenthesizedLambdaExpression(bodyStatementSyntax);

            var parameters = expr.getParameters().ToList<Parameter>();
            if (parameters != null && parameters.Count > 0)
            {
                var paramSyntaxes = new List<ParameterSyntax>();

                foreach (var param in parameters)
                {
                    string typeName = TypeHelper.ConvertType(param.getType().toString());
                    string identifier = TypeHelper.ConvertIdentifierName(param.getId().getName());

                    if ((param.getId().getArrayCount() > 0 && !typeName.EndsWith("[]")) || param.isVarArgs())
                        typeName += "[]";

                    SyntaxTokenList modifiers = SyntaxFactory.TokenList();

                    if (param.isVarArgs())
                        modifiers = SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ParamsKeyword));

                    var paramSyntax = SyntaxFactory.Parameter(
                        attributeLists: new SyntaxList<AttributeListSyntax>(),
                        modifiers: modifiers,
                        type: SyntaxFactory.ParseTypeName(typeName),
                        identifier: SyntaxFactory.ParseToken(identifier),
                        @default: null);

                    paramSyntaxes.Add(paramSyntax);
                }

                lambdaExpressionSyntax = lambdaExpressionSyntax.AddParameterListParameters(paramSyntaxes.ToArray());
            }

            return lambdaExpressionSyntax;
        }
    }
}