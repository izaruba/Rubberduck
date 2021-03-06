using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Antlr4.Runtime;
using Rubberduck.Inspections.Abstract;
using Rubberduck.Inspections.Results;
using Rubberduck.Parsing;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.Resources.Inspections;
using Rubberduck.Parsing.Symbols;
using Rubberduck.Parsing.VBA;

namespace Rubberduck.Inspections.Concrete
{
    [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
    public sealed class UnassignedVariableUsageInspection : InspectionBase
    {
        public UnassignedVariableUsageInspection(RubberduckParserState state)
            : base(state) { }

        protected override IEnumerable<IInspectionResult> DoGetInspectionResults()
        {
            var declarations = State.DeclarationFinder.UserDeclarations(DeclarationType.Variable)
                .Where(declaration =>
                    State.DeclarationFinder.MatchName(declaration.AsTypeName)
                        .All(d => d.DeclarationType != DeclarationType.UserDefinedType)
                    && !declaration.IsSelfAssigned
                    && !declaration.References.Any(reference => reference.IsAssignment));

            //See https://github.com/rubberduck-vba/Rubberduck/issues/2010 for why these are being excluded.
            //TODO: These need to be modified to correctly work in VB6.
            var lenFunction = BuiltInDeclarations.SingleOrDefault(s => s.DeclarationType == DeclarationType.Function && s.Scope.Equals("VBE7.DLL;VBA.Strings.Len"));
            var lenbFunction = BuiltInDeclarations.SingleOrDefault(s => s.DeclarationType == DeclarationType.Function && s.Scope.Equals("VBE7.DLL;VBA.Strings.LenB"));

            return declarations.Where(d => d.References.Any() &&
                                           !DeclarationReferencesContainsReference(lenFunction, d) &&
                                           !DeclarationReferencesContainsReference(lenbFunction, d))
                               .SelectMany(d => d.References)
                               .Where(r => !r.IsIgnoringInspectionResultFor(AnnotationName))
                               .Select(r => new IdentifierReferenceInspectionResult(this,
                                                                 string.Format(InspectionResults.UnassignedVariableUsageInspection, r.IdentifierName),
                                                                 State,
                                                                 r)).ToList();
        }

        private bool DeclarationReferencesContainsReference(Declaration parentDeclaration, Declaration target)
        {
            if (parentDeclaration == null)
            {
                return false;
            }
            
            foreach (var targetReference in target.References)
            {
                foreach (var reference in parentDeclaration.References)
                {
                    var context = (ParserRuleContext) reference.Context.Parent;
                    if (context.GetSelection().Contains(targetReference.Selection))
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
    }
}
