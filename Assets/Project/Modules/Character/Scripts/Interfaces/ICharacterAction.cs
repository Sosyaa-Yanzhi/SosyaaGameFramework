namespace GameFramework.Character
{
    public interface ICharacterAction
    {
        public delegate void Move();
        public delegate void Sprint();
        public delegate void Jump();
        public delegate void Interact();
        public delegate void Attack();
        public delegate void Reload();
        public delegate void Aim();
    }
}
