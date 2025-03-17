namespace Assets.Scripts
{
    public interface IDismissable
    {
        bool IsDismissed { get; }

        public void Dismiss();
    }
}