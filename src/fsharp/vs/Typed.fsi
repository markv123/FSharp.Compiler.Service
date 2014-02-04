﻿//----------------------------------------------------------------------------
// Copyright (c) 2002-2012 Microsoft Corporation. 
//
// This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
// copy of the license can be found in the License.html file at the root of this distribution. 
// By using this source code in any fashion, you are agreeing to be bound 
// by the terms of the Apache License, Version 2.0.
//
// You must not remove this notice, or any other, from this software.
//----------------------------------------------------------------------------

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open System.Collections.Generic
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Env
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Range

type [<Class>] FSharpSymbol = 
    /// Internal use only. 
    static member internal Create : g:TcGlobals * item:Nameres.Item -> FSharpSymbol

    member internal Item: Nameres.Item
        
    /// Get the declaration location for the symbol
    member DeclarationLocation: range option

    /// Gets the short display name for the symbol
    member DisplayName: string

    /// Get the implementation location for the symbol of it wsa declared in a signature that has an implementation
    member ImplementationLocation: range option



[<Class>]
/// Represents an assembly as seen by the F# language
type FSharpAssembly = 

    internal new : tcGlobals: TcGlobals * ccu: CcuThunk -> FSharpAssembly

    /// The qualified name of the assembly
    member QualifiedName: string 
    
    /// A hint for the code location for the assembly
    member CodeLocation: string 
      
    /// The contents of the this assembly 
    member Contents:  FSharpAssemblySignature

    /// The file name for the assembly, if any
    member FileName : string option

    /// The simple name for the assembly
    member SimpleName : string 

    /// Indicates if the assembly was generated by a type provider and is due for static linking
    member IsProviderGenerated : bool


/// Represents an inferred signature of part of an assembly as seen by the F# language
and [<Class>] FSharpAssemblySignature = 

    internal new : tcGlobals: TcGlobals * contents: ModuleOrNamespaceType -> FSharpAssemblySignature

    /// The (non-nested) module and type definitions in this signature
    member Entities:  IList<FSharpEntity>

/// Represents a type definition or module as seen by the F# language
and [<Class>] FSharpEntity = 
    inherit FSharpSymbol

    //   /// Return the FSharpEntity corresponding to a .NET type
    // static member FromType : System.Type -> FSharpEntity

    /// Get the name of the type or module, possibly with `n mangling  
    member LogicalName: string

    /// Get the compiled name of the type or module, possibly with `n mangling. This is identical to LogicalName
    /// unless the CompiledName attribute is used.
    member CompiledName: string

    /// Get the name of the type or module as displayed in F# code
    member DisplayName: string

    /// Get the path used to address the entity (e.g. "Namespace.Module1.NestedModule2"). Gives
    /// "global" for items not in a namespace.
    member AccessPath: string 

    /// Get the namespace containing the type or module, if any. Use 'None' for item not in a namespace.
    member Namespace: string option

    /// Get the fully qualified name of the type or module
    member QualifiedName: string 

    /// Get the declaration location for the type constructor 
    member DeclarationLocation: range 

    /// Indicates if the entity is a measure, type or exception abbreviation
    member IsFSharpAbbreviation   : bool

    /// Indicates if the entity is record type
    member IsFSharpRecord   : bool

    /// Indicates if the entity is union type
    member IsFSharpUnion   : bool

    /// Indicates if the entity is a struct or enum
    member IsValueType : bool

    /// Indicates if the entity is a provided type
    member IsProvided : bool

    /// Indicates if the entity is an erased provided type
    member IsProvidedAndErased : bool

    /// Indicates if the entity is a generated provided type
    member IsProvidedAndGenerated : bool

    /// Indicates if the entity is an F# module definition
    member IsFSharpModule: bool 

    /// Get the generic parameters, possibly including unit-of-measure parameters
    member GenericParameters: IList<FSharpGenericParameter>

    /// Indicates that a module is compiled to a class with the given mangled name. The mangling is reversed during lookup 
    member HasFSharpModuleSuffix : bool

    /// Indicates if the entity is a measure definition
    member IsMeasure: bool

    /// Indicates an F# exception declaration
    member IsFSharpExceptionDeclaration: bool 

    /// Indicates if this is a reference to something in an F#-compiled assembly
    member IsFSharp : bool

    /// Indicates if the entity is in an unresolved assembly 
    member IsUnresolved : bool

    /// Indicates if the type definition is a class type
    member IsClass : bool

    /// Indicates if the type definition is an enum type
    member IsEnum : bool

    /// Indicates if the type definition is a delegate type
    member IsDelegate : bool

    /// Indicates if the type definition is a delegate type
    member IsInterface : bool

    /// Indicates if the type definition is a delegate type
    member IsInterface : bool

    /// Get the in-memory XML documentation for the entity, used when code is checked in-memory
    member XmlDoc: IList<string>

      /// Get the XML documentation signature for the entity, used for .xml file lookup for compiled code
    member XmlDocSig: string

    /// Indicates if the type is implemented through a mapping to IL assembly code. This is only
    /// true for types in FSharp.Core.dll
    member HasAssemblyCodeRepresentation: bool 

    /// Indicates if the type prefers the "tycon<a,b>" syntax for display etc. 
    member UsesPrefixDisplay: bool                   

    /// Get the declared attributes for the type 
    member Attributes: IList<FSharpAttribute>     

    /// Get the declared interface implementations
    member DeclaredInterfaces : IList<FSharpType>  

    /// Get the base type, if any 
    member BaseType : FSharpType

    /// Get the properties, events and methods of a type definitions, or the functions and values of a module
    member MembersFunctionsAndValues : IList<FSharpMemberFunctionOrValue>
    [<System.Obsolete("Renamed to MembersFunctionsAndValues")>]
    member MembersOrValues : IList<FSharpMemberFunctionOrValue>

    /// Get the modules and types defined in a module, or the nested types of a type
    member NestedEntities : IList<FSharpEntity>

    /// Get the fields of the class, struct or enum 
    member RecordFields : IList<FSharpRecordField>

    /// Get the type abbreviated by an F# type abbreviation
    member AbbreviatedType   : FSharpType 

    /// Get the cases of a union type
    member UnionCases : IList<FSharpUnionCase>


    /// Indicates if the type is a delegate with the given Invoke signature 
    member FSharpDelegateSignature : FSharpDelegateSignature

      /// Get the declared accessibility of the type
    member Accessibility: FSharpAccessibility 

      /// Get the declared accessibility of the representation, not taking signatures into account 
    member RepresentationAccessibility: FSharpAccessibility

and [<Class>] FSharpDelegateSignature =
    /// Get the argument types of the delegate signature
    member DelegateArguments : IList<string option * FSharpType>

    /// Get the return type of the delegate signature
    member DelegateReturnType : FSharpType

/// Represents a union case as seen by the F# language
and [<Class>] FSharpUnionCase =
    inherit FSharpSymbol

    /// Get the name of the union case 
    member Name: string 

    /// Get the range of the name of the case 
    member DeclarationLocation : range

    /// Get the data carried by the case. 
    member UnionCaseFields: IList<FSharpRecordField>

    /// Get the type constructed by the case. Normally exactly the type of the enclosing type, sometimes an abbreviation of it 
    member ReturnType: FSharpType

    /// Get the name of the case in generated IL code 
    member CompiledName: string

    /// Get the in-memory XML documentation for the union case, used when code is checked in-memory
    member XmlDoc: IList<string>

    /// Get the XML documentation signature for .xml file lookup for the union case, used for .xml file lookup for compiled code 
    member XmlDocSig: string

    ///  Indicates if the declared visibility of the union constructor, not taking signatures into account 
    member Accessibility: FSharpAccessibility 

    /// Get the attributes for the case, attached to the generated static method to make instances of the case 
    member Attributes: IList<FSharpAttribute>

    /// Indicates if the union case is for a type in an unresolved assembly 
    member IsUnresolved : bool


/// Represents a record or union case field as seen by the F# language
and [<Class>] FSharpRecordField =

    inherit FSharpSymbol

    /// Is the field declared in F#? 
    member IsMutable: bool

    /// Get the in-memory XML documentation for the field, used when code is checked in-memory
    member XmlDoc: IList<string>

    /// Get the XML documentation signature for .xml file lookup for the field, used for .xml file lookup for compiled code
    member XmlDocSig: string

    /// Get the type of the field, w.r.t. the generic parameters of the enclosing type constructor 
    member FieldType: FSharpType

    /// Indicates a static field 
    member IsStatic: bool

    /// Indicates a compiler generated field, not visible to Intellisense or name resolution 
    member IsCompilerGenerated: bool

    /// Get the declaration location of the field 
    member DeclarationLocation: range

    /// Get the attributes attached to generated property 
    member PropertyAttributes: IList<FSharpAttribute> 

    /// Get the attributes attached to generated field 
    member FieldAttributes: IList<FSharpAttribute> 

    /// Get the name of the field 
    member Name : string

#if TODO
      /// Get the default initialization info, for static literals 
    member LiteralValue: obj 
#endif
      ///  Indicates if the declared visibility of the field, not taking signatures into account 
    member Accessibility: FSharpAccessibility 

    /// Indicates if the record field is for a type in an unresolved assembly 
    member IsUnresolved : bool

/// Indicates the accessibility of an item as seen by the F# language
and [<Class>] FSharpAccessibility = 
    /// Indicates the item has public accessibility
    member IsPublic : bool

    /// Indicates the item has private accessibility
    member IsPrivate : bool

    /// Indicates the item has internal accessibility
    member IsInternal : bool
        
and [<Class>] FSharpGenericParameter = 

    inherit FSharpSymbol

    /// Get the name of the generic parameter 
    member Name: string

    /// Get the range of the generic parameter 
    member DeclarationLocation : range 
       
    /// Indicates if this is a measure variable
    member IsMeasure : bool

    /// Get the in-memory XML documentation for the type parameter, used when code is checked in-memory
    member XmlDoc : IList<string>
       
    /// Indicates if this is a statically resolved type variable
    member IsSolveAtCompileTime : bool 

    /// Get the declared attributes of the type parameter. 
    member Attributes: IList<FSharpAttribute>                      
       
    /// Get the declared or inferred constraints for the type parameter
    member Constraints: IList<FSharpGenericParameterConstraint> 


/// Represents further information about a member constraint on a generic type parameter
and [<Class; NoEquality; NoComparison>] 
    FSharpGenericParameterMemberConstraint = 

    /// Get the types that may be used to satisfy the constraint
    member MemberSources : IList<FSharpType>

    /// Get the name of the method required by the constraint
    member MemberName : string 

    /// Indicates if the the method required by the constraint must be static
    member MemberIsStatic : bool

    /// Get the argument types of the method required by the constraint
    member MemberArgumentTypes : IList<FSharpType>

    /// Get the return type of the method required by the constraint
    member MemberReturnType : FSharpType 

/// Represents further information about a delegate constraint on a generic type parameter
and [<Class; NoEquality; NoComparison>] 
    FSharpGenericParameterDelegateConstraint = 

    /// Get the tupled argument type required by the constraint
    member DelegateTupledArgumentType : FSharpType

    /// Get the return type required by the constraint
    member DelegateReturnType : FSharpType 

/// Represents further information about a 'defaults to' constraint on a generic type parameter
and [<Class; NoEquality; NoComparison>] 
    FSharpGenericParameterDefaultsToConstraint = 

    /// Get the priority off the 'defaults to' constraint
    member DefaultsToPriority : int

    /// Get the default type associated with the 'defaults to' constraint
    member DefaultsToTarget : FSharpType

/// Represents a constraint on a generic type parameter
and [<Class; NoEquality; NoComparison>] 
    FSharpGenericParameterConstraint = 
    /// Indicates a constraint that a type is a subtype of the given type 
    member IsCoercesToConstraint : bool

    /// Gets further information about a coerces-to constraint
    member CoercesToTarget : FSharpType 

    /// Indicates a default value for an inference type variable should it be netiher generalized nor solved 
    member IsDefaultsToConstraint : bool

    /// Gets further information about a defaults-to constraint
    member DefaultsToConstraintData : FSharpGenericParameterDefaultsToConstraint

    /// Indicates a constraint that a type has a 'null' value 
    member IsSupportsNullConstraint  : bool

    /// Indicates a constraint that a type supports F# generic comparison
    member IsComparisonConstraint  : bool

    /// Indicates a constraint that a type supports F# generic equality
    member IsEqualityConstraint  : bool

    /// Indicates a constraint that a type is an unmanaged type
    member IsUnmanagedConstraint  : bool

    /// Indicates a constraint that a type has a member with the given signature 
    member IsMemberConstraint : bool

    /// Gets further information about a member constraint
    member MemberConstraintData : FSharpGenericParameterMemberConstraint

    /// Indicates a constraint that a type is a non-Nullable value type 
    member IsNonNullableValueTypeConstraint : bool
    
    /// Indicates a constraint that a type is a reference type 
    member IsReferenceTypeConstraint  : bool

    /// Indicates a constraint that is a type is a simple choice between one of the given ground types. Used by printf format strings.
    member IsSimpleChoiceConstraint : bool

    /// Gets further information about a choice constraint
    member SimpleChoices : IList<FSharpType>

    /// Indicates a constraint that a type has a parameterless constructor 
    member IsRequiresDefaultConstructorConstraint  : bool

    /// Indicates a constraint that a type is an enum with the given underlying 
    member IsEnumConstraint : bool

    /// Gets further information about an enumeration constraint
    member EnumConstraintTarget : FSharpType 
    
    /// Indicates a constraint that a type is a delegate from the given tuple of args to the given return type 
    member IsDelegateConstraint : bool

    /// Gets further information about a delegate constraint
    member DelegateConstraintData : FSharpGenericParameterDelegateConstraint


and [<RequireQualifiedAccess>] FSharpInlineAnnotation = 
   /// Indictes the value is inlined and compiled code for the function does not exist
   | PseudoValue
   /// Indictes the value is inlined but compiled code for the function still exists, e.g. to satisfy interfaces on objects, but that it is also always inlined 
   | AlwaysInline 
   /// Indictes the value is optionally inlined 
   | OptionalInline 
   /// Indictes the value is never inlined 
   | NeverInline 

and [<System.Obsolete("Renamed to FSharpMemberFunctionOrValue")>] FSharpMemberOrVal =  FSharpMemberFunctionOrValue
and [<Class>] FSharpMemberFunctionOrValue = 

    inherit FSharpSymbol

    /// Indicates if the member or value is in an unresolved assembly 
    member IsUnresolved : bool

    /// Get the enclosing entity for the definition
    member EnclosingEntity : FSharpEntity
    
    /// Get the declaration location of the member or value
    member DeclarationLocation: range
    
    /// Get the typars of the member or value
    member GenericParameters: IList<FSharpGenericParameter>

    /// Get the full type of the member or value when used as a first class value
    member FullType: FSharpType

    /// Indicates if this is a compiler generated value
    member IsCompilerGenerated : bool

    /// Get a result indicating if this is a must-inline value
    member InlineAnnotation : FSharpInlineAnnotation

    /// Indicates if this is a mutable value
    member IsMutable : bool

    // /// Get the reflection object for this member
    // [<System.Obsolete("This member does not yet return correct results for overloaded members")>]
    // member ReflectionMemberInfo :System.Reflection.MemberInfo

    /// Indicates if this is a module or member value
    member IsModuleValueOrMember : bool

    /// Indicates if this is an extension member?
    member IsExtensionMember : bool

    /// Indicates if this is a member, including extension members?
    member IsMember : bool

    /// Indicates if this is an abstract member?
    member IsDispatchSlot : bool

    /// Indicates if this is a getter method for a property
    member IsGetterMethod: bool 

    /// Indicates if this is a setter method for a property
    member IsSetterMethod: bool 

    /// Indicates if this is an instance member, when seen from F#?
    member IsInstanceMember : bool 
    
    /// Indicates if this is an implicit constructor?
    member IsImplicitConstructor : bool
    
    /// Indicates if this is an F# type function
    member IsTypeFunction : bool

    /// Indicates if this value or member is an F# active pattern
    member IsActivePattern : bool
      
      /// Get the member name in compiled code
    member CompiledName: string

      /// Get the logical name of the member
    member LogicalName: string

      /// Get the logical enclosing entity, which for an extension member is type being extended
    member LogicalEnclosingEntity: FSharpEntity

      /// Get the name as presented in F# error messages and documentation
    member DisplayName : string

    member CurriedParameterGroups : IList<IList<FSharpParameter>>

    member ReturnParameter : FSharpParameter

      /// Custom attributes attached to the value. These contain references to other values (i.e. constructors in types). Mutable to fixup  
      /// these value references after copying a colelction of values. 
    member Attributes: IList<FSharpAttribute>

    /// Get the in-memory XML documentation for the value, used when code is checked in-memory
    member XmlDoc: IList<string>

      /// XML documentation signature for the value, used for .xml file lookup for compiled code
    member XmlDocSig: string

     
#if TODO
    /// Indicates if this is "base" in "base.M(...)"
    member IsBaseValue : bool

    /// Indicates if this is the "x" in "type C() as x = ..."
    member IsConstructorThisValue : bool

    /// Indicates if this is the "x" in "member x.M = ..."
    member IsMemberThisValue : bool

    /// Indicates if this is a [<Literal>] value, and if so what value?
    member LiteralValue : obj // may be null

      /// Get the module, type or namespace where this value appears. For 
      /// an extension member this is the type being extended 
    member ApparentParent: FSharpEntity

     /// Get the module, type or namespace where this value is compiled
    member ActualParent: FSharpEntity

#endif

      /// How visible is this? 
    member Accessibility : FSharpAccessibility


and [<Class>] FSharpParameter =

    member Name: string
    member DeclarationLocation : range 
    member Type : FSharpType 
    member Attributes: IList<FSharpAttribute>


and [<Class>] FSharpType =
    /// Internal use only. Create a ground type.
    internal new : g:TcGlobals * typ:TType -> FSharpType

    /// Indicates this is a named type in an unresolved assembly 
    member IsUnresolved : bool

    /// Indicates this is an abbreviation for another type
    member IsAbbreviation : bool

    /// Get the type for which this is an abbreviation
    member AbbreviatedType : FSharpType

    /// Indicates if the type is constructed using a named entity, including array and byref types
    member IsNamedType : bool

    /// Get the named entity for a type constructed using a named entity
    member NamedEntity : FSharpEntity 

    /// Get the generic arguments for a tuple type, a function type or a type constructed using a named entity
    member GenericArguments : IList<FSharpType>
    
    /// Indicates if the type is a tuple type. The GenericArguments property returns the elements of the tuple type.
    member IsTupleType : bool

    /// Indicates if the type is a function type. The GenericArguments property returns the domain and range of the function type.
    member IsFunctionType : bool

    /// Indicates if the type is a variable type, whether declared, generalized or an inference type parameter  
    member IsGenericParameter : bool

    /// Get the generic parameter data for a generic parameter type
    member GenericParameter : FSharpGenericParameter


and [<Class>] FSharpAttribute = 
        
    /// The type of the attribute
    member AttributeType : FSharpEntity

    /// The arguments to the constructor for the attribute
    member ConstructorArguments : IList<obj>

    /// The named arguments for the attribute
    member NamedArguments : IList<string * bool * obj>

    /// Indicates if the attribute type is in an unresolved assembly 
    member IsUnresolved : bool


