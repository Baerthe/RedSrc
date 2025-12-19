namespace Interface;

using Godot;
using Core.Service;
public interface IStateService : IService
{
    StateSelection CurrentState { get; }
    ManagerSelection CurrentManager { get; }
}