namespace Player
{
    public interface IPlayerAction
    {
        public delegate void PlayerMove();
        public delegate void PlayerSprint();
        public delegate void PlayerJump();
        public delegate void PlayerInteract();
        public delegate void PlayerAttack();
        public delegate void PlayerReload();
        public delegate void PlayerAim();
    }
}
