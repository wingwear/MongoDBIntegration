using System.Activities;
using System.Activities.Statements;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Linq;

namespace UiPathTeam.MongoDB.Activities
{
    internal static class ParentConstraint
    {
        internal static Constraint CheckThatParentsAreOfType<ActivityType, ParentType1>(
            string validationMessage) where ActivityType : Activity
        {
            return CheckThatParentsAreOfType<ActivityType, ParentType1, ParentType1>(validationMessage);
        }

        internal static Constraint CheckThatParentsAreOfType<ActivityType, ParentType1, ParentType2>(string validationMessage) where ActivityType : Activity
        {
            var context = new DelegateInArgument<ValidationContext>();
            var result = new Variable<bool>();
            var element = new DelegateInArgument<ActivityType>();
            var parentList = new Variable<IEnumerable<Activity>>();
            var parentHasType = new InArgument<bool>(
                                    ctx => parentList.Get(ctx).Any() &&
                                    (parentList.Get(ctx).ToArray().Where(t => t.GetType().Equals(typeof(ParentType1)) || t.GetType().Equals(typeof(ParentType2))).Any()));

            return new Constraint<ActivityType>
            {
                Body = new ActivityAction<ActivityType, ValidationContext>
                {
                    Argument1 = element,
                    Argument2 = context,
                    Handler = new Sequence
                    {
                        Variables =
                        {
                            result,
                            parentList
                        },
                        Activities =
                        {
                            new Assign<IEnumerable<Activity>>
                            {
                                To = parentList,
                                Value = new GetParentChain
                                {
                                    ValidationContext = context
                                }
                            },
                            new If
                            {
                                Condition = parentHasType,
                                Then = new Assign<bool>
                                {
                                    Value = true,
                                    To = result
                                }
                            },
                            new AssertValidation
                            {
                                Assertion = new InArgument<bool>(result),
                                Message = new InArgument<string> (validationMessage),
                            }
                        }
                    }
                }
            };
        }
    }
}
