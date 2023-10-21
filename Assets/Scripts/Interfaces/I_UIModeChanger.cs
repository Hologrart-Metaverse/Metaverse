public enum UIMode
{
    StaticEdit,
    DynamicEdit,
    None,
}

public interface I_UIModeChanger 
{
    public void ChangeMode(UIMode mode);
}
