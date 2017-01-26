using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emersonville.Logic
{
    public enum TransportClass
    {
        BipedAnimal,
        QuadAnimal,
        Machine
    }

    public class PositionedObject
    {
        public virtual decimal X { get; set; }
        public virtual decimal Y { get; set; }
        public virtual decimal Z { get; set; }
    }

    public class Transport : PositionedObject, ITransport
    {
        public virtual Guid Id { get; set; }
        public virtual String Description { get; set; }
        public virtual TransportClass Class { get; set; }

        public virtual decimal MaxLoad { get; set; }
        public virtual decimal BaseLoad { get; set; }
        public virtual decimal Load { get; set; }
        public virtual decimal BaseFuelUse { get; set; }
        public virtual decimal MaxFuel { get; set; }
        public virtual decimal Fuel { get; set; }
        public virtual decimal MaxRate { get; set; }
        public virtual decimal Rate { get; set; }
        public virtual decimal EffectiveRate { get; set; }
        public virtual decimal DirectionX { get; set; }
        public virtual decimal DirectionY { get; set; }
        public virtual decimal DirectionZ { get; set; }

        protected Transport()
        {
            InitializeDynamicProperties();
        }

        protected virtual void InitializeDynamicProperties()
        {

            DirectionX = 1;
            DirectionY = 0;
            DirectionZ = 0;

            X = 0;
            Y = 0;
            Z = 0;
        }

        // for each time unit, 
        // calculate movement, rate, fuel use
        public virtual void UpdatePosition(decimal milliseconds)
        {
            EffectiveRate = Math.Max(EffectiveRate, 0);
            EffectiveRate = Math.Min(EffectiveRate, MaxRate);

            // fuel is used as a function of load and rate
            // need to calculate energy
            var fuelUsed = BaseFuelUse * ((BaseLoad + Load) / MaxLoad) * (EffectiveRate / MaxRate);
            if (fuelUsed > Fuel)
            {
                Fuel = 0;
                Rate = 0;
            }
            else
            {
                Fuel -= fuelUsed;
            }

            // need to calculate momentum
            X += (EffectiveRate * DirectionX);
            Y += (EffectiveRate * DirectionY);
            Z += (EffectiveRate * DirectionZ);
        }
    }

    public interface ITransport
    {
        Guid Id { get; set; }
        String Description { get; set; }
        TransportClass Class { get; set; }

        decimal MaxLoad { get; set; }
        decimal BaseLoad { get; set; }
        decimal Load { get; set; }

        decimal BaseFuelUse { get; set; }
        decimal MaxFuel { get; set; }
        decimal Fuel { get; set; }

        decimal MaxRate { get; set; }
        decimal Rate { get; set; }

        decimal DirectionX { get; set; }
        decimal DirectionY { get; set; }
        decimal DirectionZ { get; set; }

        void UpdatePosition(decimal milliseconds);
    }

    public interface IAnimalTransport : ITransport
    {
        decimal Effort { get; set; }
        decimal EffectiveEffort { get; set; }
        decimal Energy { get; set; }
        decimal EnergyDrag { get; set; }
        decimal EnergyRecovery { get; set; }
        decimal HealthDrag { get; set; }
        decimal HealthRecovery { get; set; }
    }

    public class AnimalTransport : Transport, ILivingOrganism, IAnimalTransport
    {
        // 0.0 to 1.0;
        public virtual decimal Energy { get; set; }
        // 0.0 to 1.0;
        public virtual decimal EnergyDrag { get; set; }
        // 0.0 to 1.0;
        public virtual decimal EnergyRecovery { get; set; }
        // 0.0 to 1.0;
        public virtual decimal Effort { get; set; }
        // 0.0 to 1.0;
        public virtual decimal EffectiveEffort { get; set; }
        // 0.0 to 1.0;
        public decimal Health { get; set; }
        // 0.0 to 1.0;
        public virtual decimal HealthDrag { get; set; }
        // 0.0 to 1.0;
        public virtual decimal HealthRecovery { get; set; }

        public decimal Age { get; set; }

        public AnimalTransport()
        {

        }

        protected override void InitializeDynamicProperties()
        {
            EnergyRecovery = .005M;
            HealthRecovery = .005M;

            base.InitializeDynamicProperties();
        }

        public override void UpdatePosition(decimal milliseconds)
        {
            Energy = Math.Min(Energy + EnergyRecovery, 1.0M);
            Health = Math.Min(Health + HealthRecovery, 1.0M);
            // Rate is requested rate, EffectiveRate is actual rate.
            if (Rate > 0.0M)
            {
                if (Health > 0 && Energy > 0)
                {
                    var rateBuffer = Rate;
                    Effort = rateBuffer / MaxRate;

                    HealthDrag = 1 - Health;
                    EnergyDrag = 1 - Energy;

                    // need to fix this here.. sends effortModifier > 1
                    var effortModifier = (1 - (HealthDrag * EnergyDrag * .025M));

                    EffectiveEffort = Effort * effortModifier;

                    EffectiveRate = Rate * EffectiveEffort;

                    EffectiveRate = Math.Max(EffectiveRate, 0);
                    EffectiveRate = Math.Min(EffectiveRate, MaxRate);

                    // reduce energy by a tenth of the effort.
                    Energy = Energy - (Energy * (EffectiveEffort * .1M));

                    // if tired and working hard, add an effect to health.
                    if (Energy < .75M)
                    {
                        if (Effort > .5M)
                        {
                            Health = Health * .98M;
                        }
                    }
                    else if (Energy < .1M)
                    {
                        Health = Health * .95M;
                    }
                }
            }
            base.UpdatePosition(milliseconds);
        }
    }

    public class Horse : AnimalTransport
    {
        public Horse()
        {
            Id = Guid.NewGuid();
            Description = "Horse";
            Class = TransportClass.QuadAnimal;

            MaxLoad = 2000;
            BaseLoad = 1000;

            BaseFuelUse = .25M;
            MaxFuel = 100;

            MaxRate = 25;

        }

        protected override void InitializeDynamicProperties()
        {
            base.InitializeDynamicProperties();

            Energy = 1.0M;
            Health = 1.0M;

            Load = 0;

            Fuel = 10;

            Rate = 0;
        }
    }
}
