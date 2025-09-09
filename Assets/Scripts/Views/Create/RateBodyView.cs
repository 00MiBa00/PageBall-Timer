using System;
using System.Collections.Generic;
using UnityEngine;
using Views.General;

namespace Views.Create
{
    public class RateBodyView : MonoBehaviour
    {
        public Action<int> PressBtnAction { get; set; }
        
        [SerializeField]
        private List<SelectorBtn> _selectorBtns;

        private void OnEnable()
        {
            for (int i = 0; i < _selectorBtns.Count; i++)
            {
                _selectorBtns[i].PressBtnAction += OnPressRateBtn;
                _selectorBtns[i].SetDefaultState();
            }
        }

        private void OnDisable()
        {
            foreach (var btn in _selectorBtns)
            {
                btn.PressBtnAction -= OnPressRateBtn;
            }
        }

        public void SetStates(int index)
        {
            for (int i = 0; i < _selectorBtns.Count; i++)
            {
                if (i < index)
                {
                    _selectorBtns[i].SetActiveState();
                }
                else
                {
                    _selectorBtns[i].SetDefaultState();
                }
            }
        }

        private void OnPressRateBtn(BtnView btn)
        {
            int index = _selectorBtns.IndexOf((SelectorBtn)btn);
            
            PressBtnAction?.Invoke(index);
        }
    }
}