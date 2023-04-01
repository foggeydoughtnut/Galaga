using System;
using System.Collections.Generic;

namespace Galaga.Systems;

public abstract class ObjectSystem : System
{
    public List<Object> Objects;
    public abstract void ObjectHit(Guid id);
}