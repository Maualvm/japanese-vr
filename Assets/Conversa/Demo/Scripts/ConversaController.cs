using Conversa.Runtime;
using Conversa.Runtime.Events;
using Conversa.Runtime.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Conversa.Demo.Scripts
{
	public class ConversaController : MonoBehaviour
	{
		[SerializeField] private Conversation conversation;
		[SerializeField] private UIController uiController;
		[SerializeField] private GameObject losingWindow;
		[SerializeField] private GameObject canvas;

		[Header("Buttons")]
		[SerializeField] private Button restartConversationButton;

		private ConversationRunner runner;
		private string savepointGuid = string.Empty;
		[SerializeField] Animator npcAnimator;
		int nameAnsweredCorrectlyHash;
		int conversationEndedHappyHash;
		int npcTalkingHash;
		int incorrectAnswerHash;
		int correctAnswerHash;
		int dismissedHash;
		int wavingHash;

		private void Start()
		{
			runner = new ConversationRunner(conversation);
			runner.OnConversationEvent.AddListener(HandleConversationEvent);
			restartConversationButton.onClick.AddListener(HandleRestartConversation);
			nameAnsweredCorrectlyHash = Animator.StringToHash("nameAnsweredCorrectly");
			conversationEndedHappyHash = Animator.StringToHash("conversationEndedHappy");
			incorrectAnswerHash = Animator.StringToHash("incorrectAnswer");
			correctAnswerHash = Animator.StringToHash("correctAnswer");
			npcTalkingHash = Animator.StringToHash("npcTalking");
			dismissedHash = Animator.StringToHash("dismissed");
			wavingHash = Animator.StringToHash("waving");
		}

		private void HandleConversationEvent(IConversationEvent e)
		{
			switch (e)
			{
				case MessageEvent messageEvent:
					HandleMessage(messageEvent);
					break;
				case ChoiceEvent choiceEvent:
					HandleChoice(choiceEvent);
					break;
				case ActorMessageEvent actorMessageEvent:
					HandleActorMessageEvent(actorMessageEvent);
					break;
				case ActorChoiceEvent actorChoiceEvent:
					HandleActorChoiceEvent(actorChoiceEvent);
					break;
				case UserEvent userEvent:
					HandleUserEvent(userEvent);
					break;
				case EndEvent _:
					HandleEnd();
					break;
			}
		}

		private void HandleActorMessageEvent(ActorMessageEvent evt)
		{
			
			var actorDisplayName = evt.Actor == null ? "" : evt.Actor.DisplayName;
			if (evt.Actor is AvatarActor avatarActor)
				uiController.ShowMessage(actorDisplayName, evt.Message, avatarActor.Avatar, evt.Advance);
			else
				uiController.ShowMessage(actorDisplayName, evt.Message, null, evt.Advance);
		}

		private void HandleActorChoiceEvent(ActorChoiceEvent evt)
		{
			var actorDisplayName = evt.Actor == null ? "" : evt.Actor.DisplayName;
			if (evt.Actor is AvatarActor avatarActor)
				uiController.ShowChoice(actorDisplayName, evt.Message, avatarActor.Avatar, evt.Options);
			else
				uiController.ShowChoice(actorDisplayName, evt.Message, null, evt.Options);
		}

		private void HandleMessage(MessageEvent e) => uiController.ShowMessage(e.Actor, e.Message, null, () => e.Advance());

		private void HandleChoice(ChoiceEvent e) => uiController.ShowChoice(e.Actor, e.Message, null, e.Options);

		private void HandleUserEvent(UserEvent userEvent)
		{
			bool isBowing = npcAnimator.GetBool(nameAnsweredCorrectlyHash);


			switch (userEvent.Name)
			{
				case "Start Angry State":
				npcAnimator.SetBool(incorrectAnswerHash, true);
				break;

				case "Finish Angry State":
				npcAnimator.SetBool(incorrectAnswerHash, false);
				break;

				case "Start Bow":
				npcAnimator.SetBool(nameAnsweredCorrectlyHash, true);
				break;

				case "Finish Bow":
				npcAnimator.SetBool(nameAnsweredCorrectlyHash, false);
				break;

				case "Start Correct Answer":
				npcAnimator.SetBool(correctAnswerHash, true);
				break;

				case "Finish Correct Answer":
				npcAnimator.SetBool(correctAnswerHash, false);
				break;

				case "Start Talking":
				npcAnimator.SetBool(npcTalkingHash, true);
				break;

				case "Finish Talking":
				npcAnimator.SetBool(npcTalkingHash, false);
				break;

				case "Start Dismiss":
				npcAnimator.SetBool(dismissedHash, true);
				break;

				case "Finish Dismiss":
				npcAnimator.SetBool(dismissedHash, false);
				break;

				case "Pass Level":
				npcAnimator.SetBool(wavingHash, true);
				restartConversationButton.gameObject.SetActive(true);
				break;

				case "Stop Waving":
				npcAnimator.SetBool(wavingHash, false);
				break;
			}
		}

		private void HandleRestartConversation()
		{

			runner.Begin();
			runner.ResetProperties();
			restartConversationButton.gameObject.SetActive(false);
		}

		private void HandleUpdateSavepoint()
		{
			savepointGuid = runner.CurrentNodeGuid;
		}

		private void HandleEnd()
		{
			uiController.Hide();
		}
	}
}
