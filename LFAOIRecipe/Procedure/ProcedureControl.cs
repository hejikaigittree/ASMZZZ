using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LFAOIRecipe
{
    class ProcedureControl : ViewModelBase
    {
        public IProcedure[] Procedures { get; protected set; }

        private object currentProcedureContent;
        public object CurrentProcedureContent
        {
            get { return currentProcedureContent; }
            set { OnPropertyChanged(ref currentProcedureContent, value); }
        }

        private int currentProcedureIndex;
        public int CurrentProcedureIndex
        {
            get { return currentProcedureIndex; }
            set { OnPropertyChanged(ref currentProcedureIndex, value); }
        }

        private bool isLastStepEnable;
        public bool IsLastStepEnable
        {
            get { return isLastStepEnable; }
            set { OnPropertyChanged(ref isLastStepEnable, value); }
        }

        private bool isNextStepEnable;
        public bool IsNextStepEnable
        {
            get { return isNextStepEnable; }
            set { OnPropertyChanged(ref isNextStepEnable, value); }
        }

        public CommandBase LastStepCommand { get; private set; }
        public CommandBase NextStepCommand { get; private set; }

        public ProcedureControl()
        {
            LastStepCommand = new CommandBase(ExecuteLastStepCommand);
            NextStepCommand = new CommandBase(ExecuteNextStepCommand);
        }

        private void ExecuteLastStepCommand(object parameter)
        {
             ProcedureChanged(CurrentProcedureIndex - 1);
        }

        private void ExecuteNextStepCommand(object parameter)
        {
            //if (ProcedureComponents[CurrentProcedureIndex].HasCompleted != null)
            //{
            //    if (!ProcedureComponents[CurrentProcedureIndex].HasCompleted.Invoke())
            //    {
            //        return;
            //    }
            //}
            //if (!Procedures[CurrentProcedureIndex].CheckCompleted()) return;
            //取消注释
            //if (!Procedures[CurrentProcedureIndex].CheckCompleted()) return;

             ProcedureChanged(CurrentProcedureIndex + 1);
        }

        protected void ProcedureChanged(int procedureIndex)
        {
            if (procedureIndex == 0)
            {
                IsLastStepEnable = false;
            }
            else
            {
                IsLastStepEnable = true;
            }
            if (procedureIndex == Procedures.Count() - 1)
            {
                IsNextStepEnable = false;
            }
            else
            {
                IsNextStepEnable = true;
            }
            CurrentProcedureIndex = procedureIndex;
            CurrentProcedureContent = Procedures[CurrentProcedureIndex].Content;
            Procedures[CurrentProcedureIndex].Initial();
        }
    }
}
