using System;
using System.Collections.Generic;
using System.Linq;
using Rubberduck.Inspections.Concrete;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Inspections.Abstract;
using Rubberduck.Parsing.Inspections.Resources;
using Rubberduck.Parsing.VBA;

namespace Rubberduck.Inspections.QuickFixes
{
    public class ChangeDimToPrivateQuickFix : IQuickFix
    {
        private static readonly HashSet<Type> _supportedInspections = new HashSet<Type> { typeof(ModuleScopeDimKeywordInspection) };
        private readonly RubberduckParserState _state;

        public ChangeDimToPrivateQuickFix(RubberduckParserState state)
        {
            _state = state;
        }
        
        public static IReadOnlyCollection<Type> SupportedInspections => _supportedInspections.ToList();

        public static void AddSupportedInspectionType(Type inspectionType)
        {
            if (!inspectionType.GetInterfaces().Contains(typeof(IInspection)))
            {
                throw new ArgumentException("Type must implement IInspection", nameof(inspectionType));
            }

            _supportedInspections.Add(inspectionType);
        }

        public void Fix(IInspectionResult result)
        {
            var rewriter = _state.GetRewriter(result.Target);

            var context = (VBAParser.VariableStmtContext)result.Target.Context.Parent.Parent;
            rewriter.Replace(context.DIM(), Tokens.Private);
        }

        public string Description(IInspectionResult result)
        {
            return InspectionsUI.ChangeDimToPrivateQuickFix;
        }

        public bool CanFixInProcedure { get; } = false;
        public bool CanFixInModule { get; } = true;
        public bool CanFixInProject { get; } = true;
    }
}