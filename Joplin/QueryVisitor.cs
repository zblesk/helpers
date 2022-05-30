using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace zblesk.Joplin;

public class QueryVisitor : ExpressionVisitor
{
    private QueryParameters parameters = new();

    (bool isProperty, string value, Type? type) GetVal(Expression ex)
        => ex is MemberExpression
        ? (true, (ex as MemberExpression)!.Member.Name!, (ex as MemberExpression)!.Expression!.Type)
        : (false, (ex as ConstantExpression)!.Value!.ToString()!, null);

    public QueryParameters ExtractParams(Expression e)
    {
        parameters = new();
        Visit(e);
        return parameters;
    }

    protected override Expression VisitMethodCall(MethodCallExpression expr)
    {
        // Console.WriteLine($"{expr.Method.Name}");

        if (new[] { "First", "FirstOrDefault" }.Contains(expr.Method.Name))
        {
            parameters.page = 1;
            parameters.limit = 1;
            parameters.RequestedResultKind = ResultKind.Single;
        }
        else if (expr.Method.Name == "Any")
        {
            parameters.page = 1;
            parameters.limit = 1;
            parameters.RequestedResultKind = ResultKind.Bool;
        }
        else if (expr.Method.Name == "Skip")
        {
            var (_, value, _) = GetVal(expr.Arguments[1]);
            parameters.skip = int.Parse(value);
        }
        else if (expr.Method.Name == "Take")
        {
            var (_, value, _) = GetVal(expr.Arguments[1]);
            parameters.take = int.Parse(value);
        }
        else if (new[] { "Where", "ToList" }.Contains(expr.Method.Name))
        {
            // do nothing
        }
        else if (new[] { "Select" }.Contains(expr.Method.Name))
        {
            parameters.ReturnType = expr.Type.GetGenericArguments().First();
        }
        else
            throw new InvalidOperationException($"Unsupported method: {expr.Method.Name}");
        return base.VisitMethodCall(expr);
    }

    protected override Expression VisitBinary(BinaryExpression expr)
    {

        if (!new[] { ExpressionType.And, ExpressionType.AndAlso, ExpressionType.Equal }.Contains(expr.NodeType))
        {
            throw new InvalidOperationException($"Invalid operator: {expr.NodeType}");
        }

        Expression left = this.Visit(expr.Left);
        Expression right = this.Visit(expr.Right);

        if (left is MemberExpression || left is ConstantExpression)
        {
            var leftVal = GetVal(left);
            var rightVal = GetVal(right);

            if (!leftVal.isProperty)
            {
                var t = leftVal;
                leftVal = rightVal;
                rightVal = t;
            }

            parameters.filterValues[leftVal.value] = rightVal.value;
            parameters.queriedTypes.Add(leftVal.type);

            // Console.WriteLine($"  -> ({leftVal}) {expr.NodeType} ('{rightVal}')");
        }
        else
        {
            // Console.WriteLine($"({left}) {expr.NodeType} ('{right}')");
        }
        return base.VisitBinary(expr);
    }

    protected override Expression VisitNew(NewExpression expr)
    {
        if (expr.Members != null)
        {
            // Console.WriteLine("  -> VisitNew " + expr.Members.Aggregate("", (a, m) => $"{a} {m.Name},"));
            foreach (var element in expr.Members)
            {
                parameters.selecting.Add(element.Name);
            }
        }
        return base.VisitNew(expr);
    }

    protected override MemberBinding VisitMemberBinding(MemberBinding node)
    {
        // Console.WriteLine($"  -> VisitMemberBinding {node.Member.Name}");
        parameters.selecting.Add(node.Member.Name);
        return base.VisitMemberBinding(node);
    }




    // ------------------------

    public override Expression Visit(Expression node)
    {
        //if (visited.Contains(node))
        //   // // Console.WriteLine($"Already saw {node}");
        return base.Visit(node);
    }

    protected override Expression VisitBlock(BlockExpression expr)
    {
        // Console.WriteLine("VisitBlock");
        return base.VisitBlock(expr);
    }

    protected override CatchBlock VisitCatchBlock(CatchBlock node)
    {
        // Console.WriteLine("VisitCatchBlock");
        return base.VisitCatchBlock(node);
    }

    protected override Expression VisitConditional(ConditionalExpression expr)
    {
        // Console.WriteLine("VisitConditional");
        return base.VisitConditional(expr);
    }

    protected override Expression VisitConstant(ConstantExpression expr)
    {
        // Console.WriteLine($"Constant: '{expr.Value}'");
        return base.VisitConstant(expr);
    }

    protected override Expression VisitDebugInfo(DebugInfoExpression expr)
    {
        // Console.WriteLine("VisitDebugInfo");
        return base.VisitDebugInfo(expr);
    }

    protected override Expression VisitDefault(DefaultExpression expr)
    {
        // Console.WriteLine("VisitDefault");
        return base.VisitDefault(expr);
    }

    protected override Expression VisitDynamic(DynamicExpression expr)
    {
        // Console.WriteLine("VisitDynamic");
        return base.VisitDynamic(expr);
    }

    protected override ElementInit VisitElementInit(ElementInit expr)
    {
        // Console.WriteLine("VisitElementInit");
        return base.VisitElementInit(expr);
    }

    protected override Expression VisitExtension(Expression expr)
    {
        // Console.WriteLine("VisitExtension");
        return base.VisitExtension(expr);
    }

    protected override Expression VisitGoto(GotoExpression expr)
    {
        // Console.WriteLine("VisitGoto");
        return base.VisitGoto(expr);
    }

    protected override Expression VisitIndex(IndexExpression expr)
    {
        // Console.WriteLine("VisitIndex");
        return base.VisitIndex(expr);
    }

    protected override Expression VisitInvocation(InvocationExpression expr)
    {
        // Console.WriteLine("VisitInvocation");
        return base.VisitInvocation(expr);
    }

    protected override Expression VisitLabel(LabelExpression expr)
    {
        // Console.WriteLine("VisitLabel");
        return base.VisitLabel(expr);
    }

    protected override LabelTarget VisitLabelTarget(LabelTarget expr)
    {
        // Console.WriteLine("VisitLabelTarget");
        return base.VisitLabelTarget(expr);
    }

    protected override Expression VisitLambda<T>(Expression<T> expr)
    {
        // Console.WriteLine("VisitLambda");
        return base.VisitLambda(expr);
    }

    protected override Expression VisitListInit(ListInitExpression expr)
    {
        // Console.WriteLine("VisitListInit");
        return base.VisitListInit(expr);
    }

    protected override Expression VisitLoop(LoopExpression expr)
    {
        // Console.WriteLine("VisitLoop");
        return base.VisitLoop(expr);
    }

    protected override Expression VisitMember(MemberExpression expr)
    {
        // Console.WriteLine($"Member {expr.Member.Name}, type {expr.Member.MemberType}");
        //return Expression.make(expr.Member.Name);
        //	return Expression.Constant(expr.Member.Name);
        return base.VisitMember(expr);
    }

    protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
    {
        // Console.WriteLine($"VisitMemberAssignment {node.Member.Name}");
        return base.VisitMemberAssignment(node);
    }

    protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
    {
        // Console.WriteLine("VisitMemberListBinding");
        return base.VisitMemberListBinding(node);
    }

    protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
    {
        // Console.WriteLine("VisitMemberMemberBinding");
        return base.VisitMemberMemberBinding(node);
    }

    protected override SwitchCase VisitSwitchCase(SwitchCase node)
    {
        // Console.WriteLine("VisitSwitchCase");
        return base.VisitSwitchCase(node);
    }

    protected override Expression VisitMemberInit(MemberInitExpression expr)
    {
        // Console.WriteLine("VisitMemberInit");
        return base.VisitMemberInit(expr);
    }

    protected override Expression VisitNewArray(NewArrayExpression expr)
    {
        // Console.WriteLine("VisitNewArray");
        return base.VisitNewArray(expr);
    }

    protected override Expression VisitParameter(ParameterExpression expr)
    {
        //// Console.WriteLine("VisitParameter");
        return base.VisitParameter(expr);
    }

    protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression expr)
    {
        // Console.WriteLine("VisitRuntimeVariables");
        return base.VisitRuntimeVariables(expr);
    }

    protected override Expression VisitSwitch(SwitchExpression expr)
    {
        // Console.WriteLine("VisitSwitch");
        return base.VisitSwitch(expr);
    }

    protected override Expression VisitTry(TryExpression expr)
    {
        // Console.WriteLine("VisitTry");
        return base.VisitTry(expr);
    }

    protected override Expression VisitTypeBinary(TypeBinaryExpression expr)
    {
        // Console.WriteLine("VisitTypeBinary");
        return base.VisitTypeBinary(expr);
    }

    protected override Expression VisitUnary(UnaryExpression expr)
    {
        // // Console.WriteLine("VisitUnary");
        return base.VisitUnary(expr);
    }
}
