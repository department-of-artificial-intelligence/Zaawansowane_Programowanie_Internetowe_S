using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contacts.Application.Domain;

public class Category(string name, int id)
{
    public Category(string name) 
        : this(name, 0)
    {

    }

    public int Id {get; private set;} = id;
    public string Name{get; private set;} = name;
}