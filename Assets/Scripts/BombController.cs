using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BombController : MonoBehaviour
{
    [Header("Bomb")]
    public KeyCode inputKey = KeyCode.LeftShift;
    public GameObject bombPrefab;
    public float bombFuseTime = 3f;
    public int bombAmount = 3;
    private int bombsRemaining;

    [Header("Explosion")]
    public Explosion explosionPrefab;
    public LayerMask explosionLayerMask;
    public float explosionDuration = 1f;
    public int explosionRadius = 5;

    [Header("Destructible")]
    public Tilemap destructibleTiles;
    public Destructible destructiblePrefab;

    private void OnEnable()
    {
        bombsRemaining = bombAmount;
    }

    
    private void Update()
    {
        // Placing the bomb if player has remaining bomb count & pressing PlaceBomb 
        if (bombsRemaining > 0 && Input.GetKeyDown(inputKey)) {
            StartCoroutine(PlaceBomb());
        }
    }

    // PlaceBomb() will place the bomb, wait 1s (fuseTime), and explode for 1s(explosionDuration), 
    // destroying any destructible objects in the range of explosion. 
    private IEnumerator PlaceBomb()
    {
        Vector2 position = transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        //Instantiate bomb and subtract 1 from remaining bomb count 
        GameObject bomb = Instantiate(bombPrefab, position, Quaternion.identity);
        bombsRemaining--;

        //Let the bomb wait for the fuseTime(1s) 
        yield return new WaitForSeconds(bombFuseTime);

        if (!bomb) {
            yield break;
        }

        position = bomb.transform.position;
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        //After fuseTime(1s), bomb will disappear, leaving explosion for 1s 
        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(explosion.start);
        explosion.DestroyAfter(explosionDuration);

        Explode(position, Vector2.up, explosionRadius);
        Explode(position, Vector2.down, explosionRadius);
        Explode(position, Vector2.left, explosionRadius);
        Explode(position, Vector2.right, explosionRadius);

        Destroy(bomb.gameObject);
        bombsRemaining++;
    }

    private void Explode(Vector2 position, Vector2 direction, int length)
    {
        if (length <= 0) {
            return;
        }

        position += direction;

        // explosionRadius = length; didnt do anything 

        //If there is destructible objects in the way of explosion, clear the destructible object
        Collider2D overlap = Physics2D.OverlapBox(position, Vector2.one / 2f, 0f, explosionLayerMask);
        if (overlap)
        {
            
            if (overlap.name == "Bomb(Clone)") {
                Explosion explosion1 = Instantiate(explosionPrefab, position, Quaternion.identity);
                explosion1.SetActiveRenderer(explosion1.start);
                explosion1.DestroyAfter(explosionDuration);
                
                Explode(position, Vector2.up, explosionRadius);
                Explode(position, Vector2.down, explosionRadius);
                Explode(position, Vector2.left, explosionRadius);
                Explode(position, Vector2.right, explosionRadius);
                GameObject bomb = overlap.gameObject;
                Destroy(bomb.gameObject);
                bombsRemaining++;
                return;
            }
            
            ClearDestructible(position);
            return;
        }

        

        Explosion explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.SetActiveRenderer(length > 1 ? explosion.middle : explosion.end);
        explosion.SetDirection(direction);
        explosion.DestroyAfter(explosionDuration);

        //If there is bomb in the way of explosion, that bomb explodes as well. 
        /* if (Physics2D.OverlapBox(position, Vector2.one * 5, 0f, explosionLayerMask))
        {
            Explode(position, Vector2.up, explosionRadius);
            Explode(position, Vector2.down, explosionRadius);
            Explode(position, Vector2.left, explosionRadius);
            Explode(position, Vector2.right, explosionRadius);
            return;
        }
        Explode(position, direction, length - 1); */
        

        
    }

    private void ClearDestructible(Vector2 position)
    {
        Vector3Int cell = destructibleTiles.WorldToCell(position);
        TileBase tile = destructibleTiles.GetTile(cell);

        if (tile != null)
        {
            Instantiate(destructiblePrefab, position, Quaternion.identity);
            destructibleTiles.SetTile(cell, null);
        }
    }

    public void AddBomb()
    {
        bombAmount++;
        bombsRemaining++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Bomb")) {
            other.isTrigger = false;
        }
    }

}
