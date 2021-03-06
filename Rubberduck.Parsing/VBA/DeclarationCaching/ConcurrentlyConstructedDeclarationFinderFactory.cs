﻿using System.Collections.Generic;
using Rubberduck.Parsing.Annotations;
using Rubberduck.Parsing.Symbols;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;

namespace Rubberduck.Parsing.VBA.DeclarationCaching
{
    public class ConcurrentlyConstructedDeclarationFinderFactory : IDeclarationFinderFactory
    {
        public DeclarationFinder Create(IReadOnlyList<Declaration> declarations, IEnumerable<IAnnotation> annotations, IReadOnlyList<UnboundMemberDeclaration> unresolvedMemberDeclarations, IHostApplication hostApp)
        {
            return new ConcurrentlyConstructedDeclarationFinder(declarations, annotations, unresolvedMemberDeclarations, hostApp);
        }

        public void Release(DeclarationFinder declarationFinder)
        {
        }
    }
}
