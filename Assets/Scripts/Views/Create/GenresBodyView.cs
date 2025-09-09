using System;
using System.Collections.Generic;

using UnityEngine;
using Views.General;

namespace Views.Create
{
    public class GenresBodyView : MonoBehaviour
    {
        public Action<int> PressBtnAction { get; set; }

        [SerializeField] 
        private List<SelectorBtn> _genreBtns;

        private void OnEnable()
        {
            for (int i = 0; i < _genreBtns.Count; i++)
            {
                _genreBtns[i].PressBtnAction += OnPressBtn;
                _genreBtns[i].SetDefaultState();
            }
        }

        private void OnDisable()
        {
            foreach (var btn in _genreBtns)
            {
                btn.PressBtnAction -= OnPressBtn;
            }
        }

        public void SetBtnState(int index, bool isActive)
        {
            if (isActive)
            {
                _genreBtns[index].SetActiveState();
            }
            else
            {
                _genreBtns[index].SetDefaultState();
            }
        }

        private void OnPressBtn(BtnView btn)
        {
            int index = _genreBtns.IndexOf((SelectorBtn)btn);
            
            PressBtnAction?.Invoke(index);
        }
    }
}