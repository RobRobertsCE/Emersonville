using System;

namespace Emersonville.Logic
{
    public interface ILivingOrganism
    {
        decimal Age { get; set; }
        decimal Energy { get; set; }
        decimal Health { get; set; }
    }
}