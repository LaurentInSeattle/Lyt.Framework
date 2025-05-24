﻿namespace Lyt.Reflector.IL;

public sealed class MethodInstructionsList
{
    private static readonly Dictionary<OpCode, byte> impliedParameters =
        new()
        {
            // Load Args
            { OpCodes.Ldarg_0, 0 },
            { OpCodes.Ldarg_1, 1 },
            { OpCodes.Ldarg_2, 2 },
            { OpCodes.Ldarg_3, 3 }
        };

    private static readonly Dictionary<OpCode, byte> impliedVariables =
        new()
        {
            // Load local 
            { OpCodes.Ldloc_0, 0 },
            { OpCodes.Ldloc_1, 1 },
            { OpCodes.Ldloc_2, 2 },
            { OpCodes.Ldloc_3, 3 },

            // Store local 
            { OpCodes.Stloc_0, 0 },
            { OpCodes.Stloc_1, 1 },
            { OpCodes.Stloc_2, 2 },
            { OpCodes.Stloc_3, 3 }
        };

    private readonly List<IInstruction> instructions = [];

    /// <summary> Create an instance for the instruction of the provided method. </summary>
    /// <param name="method">The method containing the instructions.</param>
    public MethodInstructionsList(MethodBase method)
    {
        this.Method = method;
        this.Module = method.Module;
        var maybeMethodBody = method.GetMethodBody();
        if (maybeMethodBody is not null)
        {
            this.MethodBody = maybeMethodBody;
            byte[]? data = this.MethodBody.GetILAsByteArray();
            if (data is not null)
            {
                this.Data = data;
                this.DecodeInstructions(); 
            }
            else
            {
                this.IsInvalidData = true;
                throw new InvalidOperationException("Method has No IL"); 
            }
        }
        else
        {
            this.IsInvalidData = true;
            throw new InvalidOperationException("Method has No Method Body");
        }
    }

    /// <summary>Gets the method containing these instructions.</summary>
    public MethodBase Method { get; }

    /// <summary> Gets the body of the method containing these instructions. </summary>
    public MethodBody MethodBody {  get ; }

    /// <summary> Gets the module containing these instructions. </summary>
    public Module Module { get; }

    /// <summary> Gets the byte data for these instructions. </summary>
    public byte[] Data { get; }

    /// <summary> Gets the count of instructions. </summary>
    public int Count => this.instructions.Count;

    public List<IInstruction> Instructions => this.instructions;

    /// <summary> Gets the instruction at the specified zero-based index. </summary>
    /// <param name="index">The zero-based index of the instruction.</param>
    /// <returns>The instruction at the specified zero-based index.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="index"/> is less than zero or greater than or equal to
    /// the number of instructions.
    /// </exception>
    public IInstruction InstructionAt (int index) => this.instructions[index];

    /// <summary>
    /// Gets a value indicating if any problems occurred while decoding the byte
    /// data (<see cref="Data"/>) for these instructions.
    /// </summary>
    public bool IsInvalidData { get; private set; }

    /// <summary>
    /// Gets a value indicating if the specified assembly is the same
    /// as the one containing this instruction list.
    /// </summary>
    /// <param name="assembly">The assembly to test.</param>
    /// <returns>True, if the assembly is the same; otherwise, false.</returns>
    public bool IsSameAssembly(Assembly assembly) => this.Module.Assembly == assembly;

    /// <summary> Resolve field information from a metadata token. </summary>
    /// <param name="token">The metadata token.</param>
    /// <returns>The field information, or null .</returns>
    public FieldInfo? ResolveField(Token token)
    {
        if (this.Method.DeclaringType is Type declaringType)
        {
            return 
                this.Module.ResolveField(
                       token.Value, 
                       declaringType.GetGenericArguments(),
                       this.Method.GetGenericArguments());
        }

        return null;
    } 

    /// <summary> Resolve an instruction for a byte offset. </summary>
    /// <param name="offset">The byte offset of an instruction within this method body.</param>
    /// <returns>The instruction.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="offset"/> does not specify the valid byte offset of an
    /// instruction within this method body.
    /// </exception>
    public IInstruction ResolveInstruction(int offset)
    {
        int high = this.Count - 1;
        int low = 0;

        while (low <= high)
        {
            int mid = (low + high) >> 1;
            var candidate = (Instruction) this.InstructionAt(mid);
            int candidateOffset = candidate.Offset;

            if (offset == candidateOffset)
            {
                candidate.IsTarget = true;
                return candidate;
            }

            if (low == high)
            {
                break;
            }

            if (offset < candidateOffset)
            {
                high = mid - 1;
            }
            else if (offset > candidateOffset)
            {
                low = mid + 1;
            }
        }

        throw new ArgumentOutOfRangeException(nameof(offset));
    }

    /// <summary> Resolve member information from a metadata token. </summary>
    /// <param name="token">The metadata token.</param>
    /// <returns>The member information, or null.</returns>
    public MemberInfo? ResolveMember(Token token)
    {
        if (this.Method.DeclaringType is Type declaringType)
        {
            return
                this.Module.ResolveMember(
                       token.Value,
                       declaringType.GetGenericArguments(),
                       this.Method.GetGenericArguments());
        }

        return null;
    }

    /// <summary>
    /// Resolve method information from a metadata token.
    /// </summary>
    /// <param name="token">The metadata token.</param>
    /// <returns>The method information, or null.</returns>
    public MethodBase? ResolveMethod(Token token)
    {
        if (this.Method.DeclaringType is Type declaringType)
        {
            return
                this.Module.ResolveMethod(
                       token.Value,
                       declaringType.GetGenericArguments(),
                       this.Method.GetGenericArguments());
        }

        return null;
    }

    /// <summary> Resolve parameter information from the specified index. </summary>
    /// <param name="operand">The operand for the parameter.</param>
    /// <returns>The parameter information or null.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="operand"/> does not identify a valid parameter for this method.
    /// </exception>
    public ParameterInfo? ResolveParameter(int operand)
    {
        if ((this.Method.CallingConvention & CallingConventions.HasThis) != 0 && operand-- == 0)
        {
            return null; // The "this" argument
        }

        ParameterInfo[] parameters = this.Method.GetParameters();
        if (operand < 0 || operand > parameters.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(operand));
        }

        return parameters[operand];
    }

    /// <summary> Resolve signature information from a metadata token. </summary>
    /// <param name="token">The metadata token.</param>
    /// <returns>The signature information as an array of bytes. </returns>
    public byte[] ResolveSignature(Token token) => this.Module.ResolveSignature(token.Value);

    /// <summary> Resolve string information from a metadata token. </summary>
    /// <param name="token">The metadata token.</param>
    /// <returns>The string information.</returns>
    public string ResolveString(Token token) => this.Module.ResolveString(token.Value);

    /// <summary> Resolve type information from a metadata token. </summary>
    /// <param name="token">The metadata token.</param>
    /// <returns>The type information, or null.</returns>
    public Type? ResolveType(Token token)
    {
        if (this.Method.DeclaringType is Type declaringType)
        {
            return
                this.Module.ResolveType(
                       token.Value,
                       declaringType.GetGenericArguments(),
                       this.Method.GetGenericArguments());
        }

        return null;
    }

    /// <summary> Resolve local variable information from the specified index. </summary>
    /// <param name="operand">The zero-based index of the variable.</param>
    /// <returns>The local variable information.</returns>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// <paramref name="operand"/> does not identify a valid local variable for this method.
    /// </exception>
    public LocalVariableInfo ResolveVariable(int operand)
    {
        if (operand < 0 || operand > this.MethodBody.LocalVariables.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(operand));
        }

        return this.MethodBody.LocalVariables[operand];
    }

    /// <summary> Decode the instructions. </summary>
    private void DecodeInstructions()
    {
        int count = this.Data.Length;
        int offset = 0;
        while (offset < count)
        {
            if (this.TryCreate(ref offset, out IInstruction? instruction) && instruction is not null )
            {
                this.instructions.Add(instruction);
            }
            else
            {
                this.IsInvalidData = true;
                break;
            }
        }

        this.ResolveAllInstructions();
    }
     
    // Create a switch instruction
    private SwitchInstruction CreateSwitch(int offset, OpCode opCode, ref int operandOffset)
    {
        int length = this.Data.ReadInt32(operandOffset);
        if (length < 0)
        {
            throw new ArgumentException(null, nameof(operandOffset));
        }

        int[] branches = new int[length];
        operandOffset += sizeof(int);
        for (int index = 0; index < length; index++)
        {
            branches[index] = this.Data.ReadInt32(operandOffset);
            operandOffset += sizeof(int);
        }

        return new SwitchInstruction(this, offset, opCode, length, branches);
    }

    private IInstruction CreateInstructionyByReadingToken(int offset, OpCode opCode, int operandOffset)
    {
        // Try to create a token for the specified instruction
        Token token = this.Data.ReadToken(operandOffset);
        return token.Type switch
        {
            TokenType.TypeDef or TokenType.TypeRef or TokenType.TypeSpec => new TypeInstruction(this, offset, opCode, token),
            TokenType.MethodSpec or TokenType.MethodDef => new MethodInstruction(this, offset, opCode, token),
            TokenType.FieldDef => new FieldInstruction(this, offset, opCode, token),
            TokenType.Signature => new SignatureInstruction(this, offset, opCode, token),
            TokenType.String => new StringInstruction(this, offset, opCode, token),
            TokenType.MemberRef => MemberInstruction.Create(this, offset, opCode, token),
            _ => new Instruction<Token>(this, offset, opCode, token),
        };
    }

    // Resolve all of the instruction values
    private void ResolveAllInstructions()
    {
        foreach (IInstruction instruction in this.instructions)
        {
            try 
            { 
                instruction.Resolve(); 
            }
            catch (Exception ex) 
            { 
                Debug.WriteLine("Failed to resolve instruction: " + ex);
                this.IsInvalidData = true; 
            }
        }
    }

    // Try to create an instruction from the byte data at the specified offset
    private bool TryCreate(ref int offset, out IInstruction? instruction)
    {
        instruction = null;
        if (offset >= this.Data.Length)
        {
            return false;
        }

        int index = offset;
        short code = this.Data.ReadOpCode(ref index);

        if (!AllOpCodes.Instance.TryGetValue(code, out OpCode opCode))
        {
            return false;
        }

        var dataException = new ArgumentException(null, nameof(offset));
        try
        {
            string? opCodeName = opCode.Name;
            switch (opCode.OperandType)
            {
                case OperandType.InlineBrTarget:
                    instruction = 
                        new BranchInstruction<int>(this, offset, opCode, this.Data.ReadInt32(index), sizeof(int));
                    index += sizeof(int);
                    break;

                case OperandType.ShortInlineBrTarget:
                    instruction = 
                        new BranchInstruction<sbyte>(this, offset, opCode, this.Data.ReadSByte(index), sizeof(sbyte));
                    index += sizeof(sbyte);
                    break;

                case OperandType.InlineField:
                    instruction = 
                        new FieldInstruction(this, offset, opCode, this.Data.ReadToken(index));
                    index += sizeof(int);
                    break;

                case OperandType.InlineI:
                    instruction = 
                        new Instruction<int>(this, offset, opCode, this.Data.ReadInt32(index));
                    index += sizeof(int);
                    break;

                case OperandType.InlineI8:
                    instruction = 
                        new Instruction<long>(this, offset, opCode,this.Data.ReadInt64(index));
                    index += sizeof(long);
                    break;

                case OperandType.ShortInlineI:
                    instruction = 
                        opCode == OpCodes.Ldc_I4_S ?
                            new Instruction<sbyte>(this, offset, opCode, this.Data.ReadSByte(index)) :
                            new Instruction<byte>(this, offset, opCode, this.Data.ReadByte(index));
                    index += sizeof(byte);
                    break;

                case OperandType.InlineMethod:
                    instruction = 
                        new MethodInstruction(this, offset, opCode, this.Data.ReadToken(index));
                    index += sizeof(int);
                    break;

                case OperandType.InlineNone:
                    if (impliedParameters.TryGetValue(opCode, out byte operand))
                    {
                        instruction = new ParameterInstruction<byte>(this, offset, opCode, operand);
                    }
                    else
                    {
                        if (impliedVariables.TryGetValue(opCode, out operand))
                        {
                            instruction = new VariableInstruction<byte>(this, offset, opCode, operand);
                        }
                        else
                        {
                            instruction = new Instruction(this, offset, opCode);
                        }
                    }
                    break;

                case OperandType.InlineR:
                    instruction = 
                        new Instruction<double>(this, offset, opCode,this.Data.ReadDouble(index));
                    index += sizeof(double);
                    break;

                case OperandType.ShortInlineR:
                    instruction = 
                        new Instruction<float>(this, offset, opCode, this.Data.ReadSingle(index));
                    index += sizeof(float);
                    break;

                case OperandType.InlineSig:
                    instruction = 
                        new SignatureInstruction(this, offset, opCode, this.Data.ReadToken(index));
                    index += sizeof(int);
                    break;

                case OperandType.InlineString:
                    instruction = 
                        new StringInstruction(this, offset, opCode, this.Data.ReadToken(index));
                    index += sizeof(int);
                    break;

                case OperandType.InlineSwitch:
                    instruction = this.CreateSwitch(offset, opCode, ref index);
                    break;

                case OperandType.InlineTok:
                    instruction = this.CreateInstructionyByReadingToken(offset, opCode, index);
                    index += sizeof(int);
                    break;

                case OperandType.InlineType:
                    instruction = 
                        new TypeInstruction(this, offset, opCode, this.Data.ReadToken(index));
                    index += sizeof(int);
                    break;

                case OperandType.InlineVar:
                    if (!string.IsNullOrWhiteSpace(opCodeName))
                    {
                        instruction = opCodeName.Contains("arg") ?
                            new ParameterInstruction<ushort>(this, offset, opCode, this.Data.ReadUInt16(index)) :
                            new VariableInstruction<ushort>(this, offset, opCode, this.Data.ReadUInt16(index));
                        index += sizeof(ushort);
                    } 
                    else
                    {
                        throw dataException;
                    }
                    break;

                case OperandType.ShortInlineVar:
                    if (!string.IsNullOrWhiteSpace(opCodeName))
                    {
                        instruction = 
                            opCodeName.Contains("arg") ?
                                new ParameterInstruction<byte>(this, offset, opCode, this.Data.ReadByte(index)) :
                                new VariableInstruction<byte>(this, offset, opCode, this.Data.ReadByte(index));
                        index += sizeof(byte);
                    }
                    else
                    {
                        throw dataException;
                    }
                    break;

                default:
                    throw dataException;
            }

            offset = index;
            return true;
        }
        catch(Exception ex) 
        {
            Debug.WriteLine("Failed to create instruction: " + ex.ToString());
            instruction = null;
            return false;
        }
    }    
}

/*
    /// <summary> Get an enumerator for these instructions. </summary>
    /// <returns>An enumerator for these instructions.</returns>
    //public IEnumerator<IInstruction> GetEnumerator() => this.instructions.GetEnumerator();

    ///// <summary>
    ///// Gets a value indicating if the specified instruction is contained
    ///// within a method that has "this" as its first argument.
    ///// </summary>
    ///// <param name="instruction">The instruction to test.</param>
    ///// <returns>True, if the containing method has a "this" argument; otherwise, false.</returns>
    //public bool HasThis(IInstruction instruction) 
    //    => (this.Method.CallingConvention & CallingConventions.HasThis) != 0;

*/ 