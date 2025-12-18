namespace Interface;
using Godot;
using Service;
public interface IStateService : IService
{
    StateSelection CurrentState { get; }
    ManagerSelection CurrentManager { get; }
}