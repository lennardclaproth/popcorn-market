from models.graph import Node, NodeMetadata
from db import graph as graph_db

def fetch_node_by_entity_id(entity_id: str) -> Node:
    node = graph_db.find_by_entity_id(entity_id)
    return node

def create_node(entity_id : str, metadata : NodeMetadata, child_entities : list[str]):
    node = Node(
            entity_id=entity_id,
            metadata=metadata,
            children = []
        )

    for entity_id in child_entities:
        node = fetch_node_by_entity_id(entity_id)
        if node is None:
            continue

        node.children.append(entity_id)

    graph_db.insert_node(node)