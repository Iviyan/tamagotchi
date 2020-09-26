using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tamagotchi.Properties;

namespace tamagotchi
{
    public static class Avatars
    {
        public class Avatar
        {
            public string name;
            public System.Drawing.Bitmap[] states;
            public Avatar(string name, System.Drawing.Bitmap[] states)
            {
                this.name = name;
                this.states = states;
            }
        }

        public static Avatar[] avatars = new Avatar[]
        {
            new Avatar("Student", new Bitmap[4] { Resources.stickman1, Resources.stickman2, Resources.stickman3, Resources.stickman4 }),
            new Avatar("Student2", new Bitmap[4] { Resources.stickman4, Resources.stickman2, Resources.stickman3, Resources.stickman4 }),
        };

    }
}
