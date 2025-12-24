namespace DscTool.Scoped.Graphs;

internal struct DfcState
{
    public bool Checked;
    public bool CheckResult;

    public DfcState()
    {
        Checked=false; CheckResult=false;
    }
}
