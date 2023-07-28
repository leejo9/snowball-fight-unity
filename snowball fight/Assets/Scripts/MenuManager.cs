using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] Menu[] menus;

    void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menuName)
    {
        for(int x = 0; x < menus.Length; x++)
        {
            if (menus[x].menuName == menuName)
            {
                menus[x].Open();
            }
            else if (menus[x].open)
            {
                CloseMenu(menus[x]);
            }
        }
    }
    public void OpenMenu(Menu menu)
    {
        for (int x = 0; x < menus.Length; x++)
        {
             if (menus[x].open)
            {
                CloseMenu(menus[x]);
            }
        }
        menu.Open();
    }
    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}

