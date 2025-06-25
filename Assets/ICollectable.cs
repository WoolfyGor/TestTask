using UnityEngine;

public interface ICollectable 
{
  int ID { get; }
  bool Collected { get; }
  void Collect();
  void SetCollected(bool collected);
}   
