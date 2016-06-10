using System.Collections.Generic;
using System.Text;
using ASD;

namespace Teksty
{

public partial class Huffman
    {

    private class Node
        {

        public char character;
        public long freq;
        public Node left, right;

        public Node(char character, long freq)
            {
            this.character = character;
            this.freq = freq;
            }

        public Node(long freq, Node left, Node right)
            {
            this.freq = freq;
            this.left = left;
            this.right = right;
            }

        }

    private Node root;
    private Dictionary<char, BitList> codesMap;

    public Huffman (string content)
        {

			// ETAP 1 - tu należy zaimplementować tworzenie drzewa Huffmana
			buildHuffmanTree(content);

			codesMap = new Dictionary<char,BitList>();
			buildCodesMap(root,new BitList());
        }

		private void buildHuffmanTree(string content)
		{
			var letterFrequency = new Dictionary<char, long>();
			for (int i = 0; i < content.Length; i++)
			{
				if (!letterFrequency.ContainsKey(content[i]))
					letterFrequency.Add(content[i], 1);
				else
					letterFrequency[content[i]]++;
			}
			PriorityQueue<Node> pq = new PriorityQueue<Node>((Node a, Node b) => { return a.freq < b.freq; });

			foreach (var letter in letterFrequency)
			{
				pq.Put(new Node(letter.Key, letter.Value));
			}

			Node first, second, adding;
			while (pq.Count > 1)
			{
				first = pq.Get();
				second = pq.Get();
				adding = new Node(first.freq + second.freq, first, second);
				pq.Put(adding);
			}

			root = pq.Get();
		}
    private void buildCodesMap(Node node, BitList code)
        {
			// ETAP 2  - tu należy zaimplementować generowanie kodów poszczególnych znaków oraz wypełnianie mapy codesMap
			if (node == null)
				return;
			if (node.left == null && node.right == null && code.Count == 0)
				codesMap.Add(node.character, new BitList("0"));
			else if (node.left == null && node.right == null)
				codesMap.Add(node.character, code);

			var tmp = new BitList(code);
			if (node.left != null)
			{
				code.Append(false);
				buildCodesMap(node.left, code);
			}
			if (node.right != null)
			{
				tmp.Append(true);
				buildCodesMap(node.right, tmp);
			}
        }

    public BitList Compress(string content)
        {
			// ETAP 2 - wykorzystując dane w codesMap należy zakodować napis przekazany jako parametr content
			if (content.Length == 1)
				return new BitList("0");

			codesMap = new Dictionary<char, BitList>();
			root = null;
			buildHuffmanTree(content);
			buildCodesMap(root, new BitList());
			BitList bl = new BitList();
			for(int i = 0; i < content.Length; i++)
			{
				bl.Append(codesMap[content[i]]);
			}	
			return bl;
        }

    public string Decompress(BitList compressed)
        {

			// ETAP 3 - należy zwrócić zdekodowane dane
			StringBuilder sb = new StringBuilder();
			var tmp = new BitList();
			int i = 0;
			while(i != compressed.Count)
			{
				Node n = root;
				if (n.character != '\0')
					i++;
				while (n.left != null && n.right != null)
				{
					if (compressed[i++])
						n = n.right;
					else
						n = n.left;
				} 
				sb.Append(n.character);
			}

			return sb.ToString();
        }

    }

}
