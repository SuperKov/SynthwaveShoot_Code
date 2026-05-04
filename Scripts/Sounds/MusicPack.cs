using UnityEngine;

[CreateAssetMenu(fileName = "New_MusicPack", menuName = "Create Music Pack")]
public class MusicPack : ScriptableObject
{
    [System.Serializable]
    public class MusicPart
    {
        public string PartName;
        public float PartStart;
        public float PartEnd;
        public bool IsLoop = true;
    }

    [SerializeField] private AudioClip _Music;
    public AudioClip Music => _Music;

    [SerializeField] private MusicPart[] MusicParts;

    public MusicPart GetMusicPart(string name)
    {
        foreach (MusicPart part in MusicParts)
            if (part.PartName == name)
                return part;
        return null;
    }
}