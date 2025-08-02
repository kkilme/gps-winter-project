public sealed class EncounterTile : AreaEventTile
{
    public override void OnTileEnter()
    {
        if (_willBeDestroyed) return;
        Managers.AreaMng.AreaState = AreaState.Encounter;
        Managers.AreaMng.CameraController.Freeze = true;

        AreaEncounter encounter = Managers.AreaMng.GetRandomEncounter();
        encounter.ShowEncounterPopup();
    }
}
