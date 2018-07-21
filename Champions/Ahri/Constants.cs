namespace LeagueSandbox.Champions.Ahri
{
    class AhriConsts
    {
        //Q Contants
        public const int Q_RANGE = 1000;
        public const int Q_BASE_DAMAGES = 15;
        public const int Q_DAMAGES_LEVEL_SCALING = 25;
        public const float Q_AP_RATIO = 0.35f;

        //W Contants
        public const int W_RANGE = 450;
        public const int W_BASE_DAMAGES = 15;
        public const int W_DAMAGES_LEVEL_SCALING = 25;
        public const float W_AP_RATIO = 0.4f;
        public const float W_EXPIRATION_TIME = 10f;

        //E Contants
        public const int E_RANGE = 1950;
        public const int E_BASE_DAMAGES = 15;
        public const int E_DAMAGES_LEVEL_SCALING = 25;
        public const float E_AP_RATIO = 0.35f;
        public const float E_CC_TIME_BASE = 0.75f;
        public const float E_CC_TIME_SCALE = 0.25f;
        public const float E_AMPLIFIED_DAMAGES = 1.2f;

        //R Constants
        public const int DASH_RANGE = 500;
        public const int DASH_SPEED = 1500;
        public const int DASH_HIT_RANGE = 750;
        public const int COOLDOWN_BETWEEN_DASHES = 1;
        public const int DASH_BASE_DAMAGES = 30;
        public const int DASH_DAMAGES_LEVEL_SCALING = 40;
        public const int DASH_BASE_CD = 125;
        public const int DASH_CD_SCALING = 15;
        public const int DASH_MANA_COST = 100;
        public const float DASH_EXPIRATION_TIME = 10f;
        public const float DASH_AP_RATIO = 0.3f;
    }
}
