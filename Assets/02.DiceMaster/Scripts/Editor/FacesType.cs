// Copyright Michele Pirovano 2014-2016

namespace DiceMaster
{
    public enum FacesType
    {
        Texture,    // Each face is drawn on a single texture
        Dynamic,    // Each face is a textured quad. Useful to have dynamic faces that can change at runtime.
        Custom, // Each face is a custom gameobject. You can attach anything to the face.
    }

}
