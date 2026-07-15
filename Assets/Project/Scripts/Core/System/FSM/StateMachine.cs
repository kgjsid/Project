using System;
using System.Collections.Generic;

namespace Core.System.FSM
{
    public class StateMachine
    {
        private IState currentState;
        private Dictionary<IState, List<(IState to, Func<bool> condition)>> transitions 
            = new Dictionary<IState, List<(IState to, Func<bool> condition)>>();
        private List<(IState to, Func<bool> condition)> anyTransitions = 
            new List<(IState to, Func<bool> condition)>();

        public void SetState(IState state)
        {
            if (currentState == state) return;

            currentState?.Exit();
            currentState = state;
            currentState.Enter();
        }

        /// <summary>
        /// 전이 조건 추가
        /// (Hit -> Die, Trace -> Attack...)
        /// </summary>
        /// <param name="from">현재 상태</param>
        /// <param name="to">목적 상태</param>
        /// <param name="condition">조건 메소드</param>
        public void AddTransition(IState from, IState to, Func<bool> condition)
        {
            if (!transitions.TryGetValue(from, out var list))
            {
                list = new List<(IState, Func<bool>)>();
                transitions[from] = list;
            }
            list.Add((to, condition));
        }

        /// <summary>
        /// 현재 상태와 관계 없이 전이하는 조건 추가
        /// (Die, Hit...)
        /// </summary>
        /// <param name="to">목적 상태</param>
        /// <param name="condition">조건 메소드</param>
        public void AddAnyTransition(IState to, Func<bool> condition)
        {
            anyTransitions.Add((to, condition));
        }

        public void Update()
        {
            // anyTransitions을 먼저 검사 -> 어떤 상태든 우선적으로 전이될 수 있게
            foreach (var t in anyTransitions)
            {
                if (t.condition())
                {
                    SetState(t.to);
                    break;
                }
            }

            // 현재 상태에 등록된 전이만 검사
            if (currentState != null && transitions.TryGetValue(currentState, out var list))
            {
                foreach (var t in list)
                {
                    if (t.condition())
                    {
                        SetState(t.to);
                        break;
                    }
                }
            }

            currentState?.Update();
        }
    }
}