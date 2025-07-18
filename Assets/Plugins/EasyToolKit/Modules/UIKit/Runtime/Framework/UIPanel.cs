using System;
using Cysharp.Threading.Tasks;
using EasyToolKit.Core;
using UnityEngine;

namespace EasyToolKit.UIKit
{
    public abstract class UIPanel : MonoBehaviour, IPanel
    {
        protected UILevel PanelLevel { get; private set; }

        protected int PanelOrder { get; private set; }

        protected PanelInfo PanelInfo { get; private set; }

        PanelInfo IPanel.Info
        {
            get => PanelInfo;
            set => PanelInfo = value;
        }

        protected PanelState PanelState { get; private set; }

        Transform IPanel.Transform => transform;
        PanelState IPanel.State => PanelState;

        int IPanel.Order
        {
            get => PanelOrder;
            set
            {
                PanelOrder = value;

                if (PanelOrder < (int)UILevel.Background)
                {
                    PanelLevel = UILevel.Lowest;
                }
                else if (PanelOrder < (int)UILevel.Common)
                {
                    PanelLevel = UILevel.Background;
                }
                else if (PanelOrder < (int)UILevel.PopUI)
                {
                    PanelLevel = UILevel.Common;
                }
                else if (PanelOrder < (int)UILevel.Topest)
                {
                    PanelLevel = UILevel.PopUI;
                }
                else
                {
                    PanelLevel = UILevel.Topest;
                }
            }
        }

        async UniTask IPanel.InitializeAsync()
        {
            PanelState = PanelState.Initializing;
            await OnInitAsync();
            PanelState = PanelState.Initialized;
        }

        void IPanel.Open(IPanelData panelData)
        {
            OnOpen(panelData);
            PanelState = PanelState.Opened;
        }

        void IPanel.Close()
        {
            if (PanelState == PanelState.Closed || PanelState == PanelState.Killed)
                return;

            OnClose();
            PanelState = PanelState.Closed;
        }

        void IPanel.Kill()
        {
            if (PanelState == PanelState.Killed)
                return;
            Assert.True(PanelState == PanelState.Closed);

            OnKill();
            Destroy(gameObject);
            PanelState = PanelState.Killed;
        }

        protected virtual UniTask OnInitAsync()
        {
            return UniTask.CompletedTask;
        }

        protected virtual void OnOpen(IPanelData panelData)
        {
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnKill()
        {
        }
    }
}
