using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Controllers.Game;
using Views.Game;
using Views.General;

using Models;
using Enums;

namespace Controllers.Scenes
{
    public class GameSceneController : AbstractSceneController
    {
        [Space(5)] [Header("Controllers")] 
        [SerializeField]
        private PreviewBooksController _previewBooksController;

        [Space(5)] [Header("Views")] 
        [SerializeField] 
        private StatusView _statusView;
        [SerializeField] 
        private AllBooksPanel _allBooksPanel;
        [SerializeField] 
        private BookDescriptionPanel _bookDescriptionPanel;
        [SerializeField] 
        private PanelView _confirmationPanel;
        [SerializeField] 
        private PanelView _privacyPanel;

        [Space(5)] [Header("Buttons")] 
        [SerializeField]
        private BtnView _progressBtn;
        [SerializeField] 
        private BtnView _rewardBtn;
        [SerializeField] 
        private BtnView _subscriptionBtn;
        [SerializeField] 
        private Button _createBtn;
        [SerializeField] 
        private Button _privacyBtn;
        
        private GameModel _model;
        private EditModel _editModel;
        
        protected override void OnSceneEnable()
        {
            SetBooks();
            SetState();
        }

        protected override void OnSceneStart()
        {
            
        }

        protected override void OnSceneDisable()
        {
            
        }

        protected override void Initialize()
        {
            _model = new GameModel();
            _editModel = new EditModel();
        }

        protected override void Subscribe()
        {
            _previewBooksController.Subscribe();
            _previewBooksController.PressAllBtnAction += OnPressAllBtn;
            _previewBooksController.PressReadBtnAction += OnPressReadBtn;
            
            _privacyBtn.onClick.AddListener(OnPressPrivacyBtn);
            _createBtn.onClick.AddListener(OnPressCreateBtn);
            
            _progressBtn.PressBtnAction += OnPressProgressBtn;
            _rewardBtn.PressBtnAction += OnPressRewardBtn;
            _subscriptionBtn.PressBtnAction += OnPressPremiumBtn;

            EditSceneController.OnEditSceneClosed += OnEditSceneClosed;
        }

        protected override void Unsubscribe()
        {
            _previewBooksController.Unsubscribe();
            _previewBooksController.PressAllBtnAction -= OnPressAllBtn;
            _previewBooksController.PressReadBtnAction -= OnPressReadBtn;
            
            _privacyBtn.onClick.RemoveAllListeners();
            _createBtn.onClick.RemoveAllListeners();
            
            _progressBtn.PressBtnAction -= OnPressProgressBtn;
            _rewardBtn.PressBtnAction -= OnPressRewardBtn;
            _subscriptionBtn.PressBtnAction -= OnPressPremiumBtn;
            
            EditSceneController.OnEditSceneClosed -= OnEditSceneClosed;
        }

        private void SetBooks()
        {
            List<BookModel> models = new List<BookModel>(_model.GetBookModels());

            _previewBooksController.Initialize(models);
        }

        private void SetState()
        {
            bool isVip = _model.IsVipActive;
            
            _statusView.UpdateStatus(_model.StatusString);

            _progressBtn.gameObject.SetActive(isVip);
            _rewardBtn.gameObject.SetActive(isVip);
            _subscriptionBtn.gameObject.SetActive(!isVip);
        }

        private void OnPressRewardBtn(BtnView btn)
        {
            base.LoadScene(SceneName.RewardScene.ToString());
        }

        private void OnPressProgressBtn(BtnView btn)
        {
            base.LoadScene(SceneName.ProgressScene.ToString());
        }

        private void OnPressPremiumBtn(BtnView btn)
        {
            base.LoadScene(SceneName.Subscription.ToString());
        }

        private void OnPressCreateBtn()
        {
            base.LoadScene(SceneName.CreateScene.ToString());
        }

        private void OnPressAllBtn()
        {
            base.SetClickClip();
            
            _allBooksPanel.SetBooks(_model.GetBookModels());
            _allBooksPanel.OnPressBtnAction += OnReceiveAnswerAllBooksPanel;
            _allBooksPanel.PressItemAction += OnPressReadBtn;
            _allBooksPanel.Open();
        }

        private void OnPressPrivacyBtn()
        {
            base.SetClickClip();
            
            _privacyPanel.OnPressBtnAction += OnReceiveAnswerPrivacyPanel;
            OpenPrivacyPanel();
        }

        private void OnReceiveAnswerPrivacyPanel(int answer)
        {
            base.SetClickClip();
            
            _privacyPanel.OnPressBtnAction -= OnReceiveAnswerPrivacyPanel;
            ClosePrivacyPanel();
        }

        private void OnPressReadBtn(int index)
        {
            base.SetClickClip();
            
            OpenDescriptionPanel(index);
        }

        private void SetActivePrivacyPanel(bool value)
        {
            if (value)
            {
                _privacyPanel.Open();
            }
            else
            {
                _privacyPanel.Close();
            }
        }

        private void OpenPrivacyPanel()
        {
            SetActivePrivacyPanel(true);
        }

        private void ClosePrivacyPanel()
        {
            SetActivePrivacyPanel(false);
        }

        private void OpenDescriptionPanel(int index)
        {
            _model.SetSelectedBookIndex(index);
            
            SetInfoDescriptionPanel();
        }

        private void OpenConfirmationPanel()
        {
            _confirmationPanel.OnPressBtnAction += OnReceiveAnswerConfirmationPanel;
            _confirmationPanel.Open();
        }

        private void OnReceiveAnswerConfirmationPanel(int answer)
        {
            base.SetClickClip();
            
            _confirmationPanel.OnPressBtnAction -= OnReceiveAnswerConfirmationPanel;

            switch (answer)
            {
                case 0:
                    DeleteBook();
                    break;
                case 1:
                    _confirmationPanel.Close();
                    break;
            }
        }

        private void OnReceiveAnswerAllBooksPanel(int answer)
        {
            base.SetClickClip();
            
            _allBooksPanel.Unsubscribe();
            
            _allBooksPanel.OnPressBtnAction -= OnReceiveAnswerAllBooksPanel;
            _allBooksPanel.PressItemAction -= OnPressReadBtn;
            
            if (answer == 0)
            {
                _allBooksPanel.Close();
                _previewBooksController.SetCheckSwipe(true);
            }
            else
            {
                base.LoadScene(SceneName.CreateScene.ToString());
            }
        }

        private void OnReceiveAnswerBookDescriptionPanel(int answer)
        {
            switch (answer)
            {
                case 0:
                    base.SetClickClip();
                    _bookDescriptionPanel.OnPressBtnAction -= OnReceiveAnswerBookDescriptionPanel;
                    _bookDescriptionPanel.Close();
                    break;
                case 1:
                    base.SetClickClip();
                    OnPressDeleteBook();
                    break;
                case 2:
                    _editModel.SetIndex(_model.SelectedBookIndex);
                    base.LoadScene(SceneName.EditScene.ToString(), false);
                    break;
            }
        }

        private void OnPressDeleteBook()
        {
            OpenConfirmationPanel();
        }

        private async void DeleteBook()
        {
            await _model.DeleteBook();

            SetBooks();
            SetState();

            List<BookModel> models = new List<BookModel>(_model.GetBookModels());

            _previewBooksController.Initialize(models);
            _allBooksPanel.SetBooks(models);
            
            _confirmationPanel.Close();
            OnReceiveAnswerBookDescriptionPanel(0);
        }

        private void OnEditSceneClosed()
        {
            UpdateDescription();
        }

        private void SetInfoDescriptionPanel()
        {
            List<BookModel> models = new List<BookModel>(_model.GetBookModels());
            _bookDescriptionPanel.SetActiveDeleteBtn(_model.SelectedBookIndex > 0);
            _bookDescriptionPanel.SetDescription(models[_model.SelectedBookIndex]);
            _bookDescriptionPanel.OnPressBtnAction += OnReceiveAnswerBookDescriptionPanel;
            _bookDescriptionPanel.Open();
        }

        private void UpdateDescription()
        {
            SetBooks();

            List<BookModel> models = new List<BookModel>(_model.GetBookModels());
            
            _allBooksPanel.SetBooks(models);
            
            SetInfoDescriptionPanel();
        }
    }
}