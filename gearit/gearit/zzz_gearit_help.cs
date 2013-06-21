/*
  // Permet de tester le type d'un objet
    if (objet.GetType() == typeof(PrismaticJoint))

  // C# getter/setter sans déclaration préalable d'attribut
    public object Size { get; set; }

  // Avec déclaration préalable
    public float Size
    {
       get { return _size; }
       set { _size = value; } //value is a keyword
    }

EXEMPLE D'AWSOME SHORTCUT ICI /!\
/////////////////////
            foreach (Fixture f in b.FixtureList)
                DrawLines(f, xf, col);
/////////////////////


    // Serialization d'objet:
    * Ajouter les "using" suivant:
       * `using System.Runtime.Serialization;`
       * `using System.Runtime.Serialization.Formatters.Binary;`
    * Ajouter `[Serializable()]` au dessus de la classe.
    * Faire heriter la classe de `ISerializable`.
    * Creer une constructeur avec les parametres `SerializationInfo info`
	  et `StreamingContext ctxt` pour setter les valeurs.
    * Creer une founction `GetObjectData` pour mettre les valeurs dans la map de
	  Serialisation.
    * Exemple:

    [Serializable()]
    class Coucou : ISerializable
    {
      int _nbBisoux;

      public Coucou(int bisoux)
      {
        this->_nbBisoux = bisoux;
      }

      public Coucou(SerializationInfo info, StreamingContext ctxt)
      {
         this->_nbBisoux = (int) info.GetValue("Bisoux", typeof(int));
      }

      public GetObjectData(SerializationInfo info, StreamingContext ctxt)
      {
        info.AddValue("Bisoux", this._nbBisoux);
      }

      [...]
    }










*/