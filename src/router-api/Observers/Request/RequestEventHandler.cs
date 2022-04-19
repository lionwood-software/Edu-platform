using System;

namespace RouterApi.Observers.Request
{
    public class RequestEventHandler : BaseEventHandler<RequestEventArgs>
    {
        private readonly RequestObserver _requestObserver;

        public RequestEventHandler(RequestObserver requestObserver)
        {
            _requestObserver = requestObserver;

            InitializeEvent();
        }

        public event EventHandler<RequestEventArgs> OnApprove;

        public event EventHandler<RequestEventArgs> OnReject;

        public override void InitializeEvent()
        {
            OnCreate += _requestObserver.SendNotificationAsync;

            OnApprove += _requestObserver.SendNotificationAsync;

            OnReject += _requestObserver.SendNotificationAsync;
            OnReject += _requestObserver.AfterRejectActionsAsync;
        }

        public void ApproveInvoke(object sender, RequestEventArgs e)
        {
            OnApprove?.Invoke(sender, e);
        }

        public void RejectInvoke(object sender, RequestEventArgs e)
        {
            OnReject?.Invoke(sender, e);
        }
    }
}
