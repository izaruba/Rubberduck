﻿using System.Diagnostics;
using System.Runtime.InteropServices.ComTypes;
using Rubberduck.Parsing.Grammar;
using TYPEATTR = System.Runtime.InteropServices.ComTypes.TYPEATTR;
using TYPEFLAGS = System.Runtime.InteropServices.ComTypes.TYPEFLAGS;

namespace Rubberduck.Parsing.ComReflection
{
    [DebuggerDisplay("{Name} As {TypeName}")]
    public class ComAlias : ComBase
    {
        public string TypeName { get; }
        public bool IsHidden { get; }
        public bool IsRestricted { get; }

        public ComAlias(IComBase parent, ITypeLib typeLib, ITypeInfo info, int index, TYPEATTR attributes) : base(parent, typeLib, index)
        {
            Index = index;
            Documentation = new ComDocumentation(typeLib, index);
            Guid = attributes.guid;
            IsHidden = attributes.wTypeFlags.HasFlag(TYPEFLAGS.TYPEFLAG_FHIDDEN);
            IsRestricted = attributes.wTypeFlags.HasFlag(TYPEFLAGS.TYPEFLAG_FRESTRICTED);
            
            if (Name.Equals("LONG_PTR"))
            {
                TypeName = Tokens.LongPtr;
                return;                
            }

            var aliased = new ComParameter(attributes, info);
            TypeName = aliased.TypeName;
        }
    }
}
