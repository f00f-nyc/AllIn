using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DT.AllIn.Data
{
    public enum ActionType
    {
        NoOp,
        TestAction,
        JoinGame,
        HireWorker,
        FireWorker,
        ProposeLoan,
        WithdrawProposal,
        GiveGift,
        AssignWorker,
        TakeLoan
    }

    public interface IGameAction
    {
        int PlayerId { get; set; }
        ActionType ActionType { get; }
    }

    public class GameAction
    {
        public static Dictionary<ActionType, Type> ActionMap = new Dictionary<ActionType, Type>
        {
            { ActionType.NoOp, typeof(NoOpAction) },
            { ActionType.TestAction, typeof(TestAction) },
            { ActionType.JoinGame, typeof(GameAction) },
            { ActionType.HireWorker, typeof(HireWorkerAction) },
            { ActionType.FireWorker, typeof(FireWorkerAction) },
            { ActionType.ProposeLoan, typeof(ProposeLoanAction) },
            { ActionType.WithdrawProposal, typeof(WithdrawProposalAction) },
            { ActionType.GiveGift, typeof(GiveGiftAction) },
            { ActionType.AssignWorker, typeof(AssignWorkerAction) },
            { ActionType.TakeLoan, typeof(TakeLoanAction) }
        };
        public static Type DefaultActionType = typeof(GameAction);

        public int PlayerId { get; set; }
        public ActionType ActionType { get; set; }
        public GameAction() { this.ActionType = ActionType.NoOp; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static GameAction FromJson(string json)
        {
            GameAction act = JsonConvert.DeserializeObject<GameAction>(json);
            Type type = GameAction.ActionMap[act.ActionType];

            if (type == GameAction.DefaultActionType)
            {
                return act;
            }

            return (GameAction)JsonConvert.DeserializeObject(json, type);
        }

        public static List<GameAction> DeserializeList(string json)
        {
            JArray actions = (JArray)JsonConvert.DeserializeObject(json);
            List<GameAction> ret = new List<GameAction>();

            foreach (var action in actions)
            {
                string actionJson = action.ToString();
                ret.Add(GameAction.FromJson(actionJson));
            }

            return ret;
        }

        public static string SerializeList(List<GameAction> actions)
        {
            return JsonConvert.SerializeObject(actions);
        }
    }

    public class NoOpAction : GameAction
    { }

    public class TestAction : GameAction
    {
        public string TestValue { get; set; }
        public TestAction() { this.ActionType = ActionType.TestAction; }
    }

    public class JoinAction : GameAction
    {
        public string PlayerName { get; set; }
        public JoinAction() { this.ActionType = ActionType.JoinGame; }
    }

    public class HireWorkerAction : GameAction
    {
        public HireWorkerAction() { this.ActionType = ActionType.HireWorker; }
    }

    public class FireWorkerAction : GameAction
    {
        public FireWorkerAction() { this.ActionType = ActionType.FireWorker; }
    }

    public class ProposeLoanAction : GameAction
    {
        public TimeSpan Period { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal PayBack { get; set; }

        public ProposeLoanAction() { this.ActionType = ActionType.ProposeLoan; }
    }

    public class WithdrawProposalAction : GameAction
    {
        public int ProposalId { get; set; }
        public WithdrawProposalAction() { this.ActionType = ActionType.WithdrawProposal; }
    }

    public class GiveGiftAction : GameAction
    {
        public decimal GiftAmount { get; set; }
        public int ToPlayerId { get; set; }
        public GiveGiftAction() { this.ActionType = ActionType.GiveGift; }
    }

    public class AssignWorkerAction : GameAction
    {
        public int WorkerId { get; set; }
        public Job Work { get; set; }
        public AssignWorkerAction() { this.ActionType = ActionType.AssignWorker; }
    }

    public class TakeLoanAction : GameAction
    {
        public int LoanProposalId { get; set; }
        public TakeLoanAction() { this.ActionType = ActionType.TakeLoan; }
    }
}
