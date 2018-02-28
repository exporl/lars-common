using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lars.Sound
{

    public class MovingFilter : FilterBase
    {

        BinauralGame game;

        public void Initialize(BinauralGame game) {
            this.game = game;
        }

        public void Finish() {
            game = null;
        }

        /// <summary>
        /// True if the filter has a valid source game to receive bias values from.
        /// </summary>
        protected bool ready { get { return game != null; } }

        /// <summary>
        /// Value between -1 and 1 indicating the bias between left and right ear.
        /// </summary>
        protected float bias { get { return game.GetBias(); } }

        /// <summary>
        /// Absolute value of how much bias should change over the next block.
        /// </summary>
        protected float deltaBias { get { return game.GetBiasChange() * (float)SECONDS_PER_BLOCK; } }
        
        /// <summary>
        /// Direction in which deltaBias should be applied.
        /// </summary>
        protected Direction audioDirection { get { return game.GetDirection(); } }
        

    }


}