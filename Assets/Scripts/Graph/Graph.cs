public class Graph<T>
{
    private Node<T>[,] _nodes;

    public Graph(int rows, int columns)
    {
        _nodes = new Node<T>[rows, columns];
    }

    public void SetNode(int x, int y, T value)
    {
        _nodes[x, y] = new Node<T>(value);
    }

    private void ConnectNodes()
    {
        var rows = _nodes.GetLength(0);
        var columns = _nodes.GetLength(1);

        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < columns; j++)
            {
                var currentNode = _nodes[i, j];

                if (i > 0)
                    currentNode.Neighbors.Add(_nodes[i - 1, j]); // Add the node above

                if (i < rows - 1)
                    currentNode.Neighbors.Add(_nodes[i + 1, j]); // Add the node below

                if (j > 0)
                    currentNode.Neighbors.Add(_nodes[i, j - 1]); // Add the node to the left

                if (j < columns - 1)
                    currentNode.Neighbors.Add(_nodes[i, j + 1]); // Add the node to the right
            }
        }
    }
}
