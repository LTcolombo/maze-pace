using System.Collections.Generic;
using System;
using UnityEngine;

namespace AssemblyCSharp
{
	class SpeedUpChain: IComparable
	{
		public List<NodeData> nodes = new List<NodeData> ();
		public int direction;
		
		int IComparable.CompareTo (object other)
		{
			return ((SpeedUpChain)other).nodes.Count - nodes.Count;
		}
	}

	public class SpeedUpDecorator
	{
		public static void Apply (MazeData mazeData)
		{
			if (mazeData.config.speedUpsCount == 0)
				return;
				
			List<SpeedUpChain> chains = new List<SpeedUpChain> ();
				
			foreach (NodeData deadEnd in mazeData.deadEnds) {
				
				NodeData node = deadEnd;
				int currentDirection = -1;
				SpeedUpChain currentChain = new SpeedUpChain ();
				
				do {
					NodeData nextNode = node.previousNode;
			
					bool nodeHasSpeedUp = false;
					foreach (SpeedUpChain chain in chains) {
						if (chain.nodes.Contains (node)) {
							nodeHasSpeedUp = true;
							break;
						}
					}
					
					if (nodeHasSpeedUp) //branch reached a point of another branch that has already been processed
						break;
									
					if (nextNode != null) {
						int direction = GetDirection (nextNode, node);
						
						currentChain.nodes.Add (node);
						if (currentDirection != direction) {
							if (currentChain != null && currentChain.nodes.Count > 1)
								chains.Add (currentChain);
								
							currentChain = new SpeedUpChain ();
							currentChain.direction = direction;
						}				
						currentDirection = direction;
					}
					node = nextNode;
				} while (node!=null);
			}
			
			chains.Sort ();
			
			for (int i =0; i < chains.Count; i++) {
				if (i >= mazeData.config.speedUpsCount) 
					break;
			
				//mark nodes to contain according speedup flags
				foreach (NodeData nodeData in chains[i].nodes) {
					switch (chains[i].direction) {
					case (NodeData.DIRECTION_UP_IDX):
						nodeData.AddFlag (NodeData.SPECIALS_SPEEDUP_UP);
						break;
						
					case (NodeData.DIRECTION_RIGHT_IDX):
						nodeData.AddFlag (NodeData.SPECIALS_SPEEDUP_RIGHT);
						break;
						
					case (NodeData.DIRECTION_DOWN_IDX):
						nodeData.AddFlag (NodeData.SPECIALS_SPEEDUP_DOWN);
						break;
						
					case (NodeData.DIRECTION_LEFT_IDX):
						nodeData.AddFlag (NodeData.SPECIALS_SPEEDUP_LEFT);
						break;
					}
				}
			}
		}

		static int GetDirection (NodeData nextNode, NodeData node)
		{
			//presumably nodes are next to each other
			//inverse direction to make speedup towards dead ends (and exit)
			if (nextNode.x == node.x) {
				if (nextNode.y < node.y)
					return NodeData.DIRECTION_UP_IDX;
				else
					return NodeData.DIRECTION_DOWN_IDX;
			} else {
				if (nextNode.x < node.x)
					return NodeData.DIRECTION_RIGHT_IDX;
				else
					return NodeData.DIRECTION_LEFT_IDX;
			}
		}
	}
}