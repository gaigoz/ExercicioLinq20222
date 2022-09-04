using Microsoft.EntityFrameworkCore;
using Persistencia.Entidades;
using Persistencia.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MoviesConsoleApp
{
    class Program
    {
        static void Main(String[] args)
        {
            // acesso ao EF serah realizado pela variavel _db
            // essa dependencia da camada de apresentacao com
            // a camada de persistencia eh errada!
            MovieContext _db = new MovieContext();

            Console.WriteLine();
            Console.WriteLine("1. Listar o nome de todos personagens desempenhados por um determinado ator, incluindo a informação de qual o título do filme e o diretor");

            String ator = "Harrison Ford";
            var query1 = from p in _db.Characters
                         where p.Actor.Name == ator            

                         select new
                         {
                            p.Movie.Title,
                            p.Movie.Director,
                            p.Character
                         };
                        

            foreach (var res in query1)
            {

                Console.WriteLine("\t {0} - {1}  - {2} ", res.Character, res.Title, res.Director);
            }


            Console.WriteLine();
            Console.WriteLine("2. Mostrar o nome e idade de todos atores que desempenharam um determinado personagem(por exemplo, quais os atores que já atuaram como '007' ?");

            String personagem = "James Bond";
            var query2 = from p in _db.Characters
                         where p.Character == personagem
                         select new
                         {
                             p.Actor.Name,
                             Age = DateTime.Now.Year - p.Actor.DateBirth.Year
                         };
            
            foreach (var res in query2)
            {

                Console.WriteLine("\t {0} - {1}", res.Name, res.Age);
            }

            Console.WriteLine();
            Console.WriteLine("3. Informar qual o ator desempenhou mais vezes um determinado personagem(por exemplo: qual o ator que realizou mais filmes como o 'agente 007'");

            var query3  = from p in _db.Characters
                           where p.Character == "James Bond"
                           group p by p.Actor.Name into grpName
                           orderby grpName.Count() descending
                           select new
                           {
                               Chave = grpName.Key,
                               Numero = grpName.Count()
                           };

            foreach (var res in query3)
            {
                Console.WriteLine("\t {0} {1}", res.Chave, res.Numero);
            }

            Console.WriteLine();
            Console.WriteLine("4. Mostrar o nome e a data de nascimento do ator mais idoso");

            var idoso = _db.Actors.Min(d => d.DateBirth);
            var query4 =  from p in _db.Characters
                          where p.Actor.DateBirth == idoso
                          select new {
                                p.Actor.Name,
                                Birth = p.Actor.DateBirth.ToShortDateString(),
                                Age = DateTime.Now.Year - p.Actor.DateBirth.Year
                          };

            foreach (var res in query4)
            {

                Console.WriteLine("\t {0} - {1} - {2}", res.Name, res.Birth, res.Age);
            }

            Console.WriteLine();
            Console.WriteLine("5. Mostrar o nome e a data de nascimento do ator mais novo a atuar em um determinado gênero");

            var novo = _db.Actors.Max(d => d.DateBirth);
            var query5 = from p in _db.Characters
                         where p.Actor.DateBirth == novo
                         select new
                         {
                             p.Actor.Name,
                             Birth = p.Actor.DateBirth.ToShortDateString(),
                             Age = DateTime.Now.Year - p.Actor.DateBirth.Year
                         };
            foreach (var res in query5)
            {

                Console.WriteLine("\t {0} - {1} - {2}", res.Name, res.Birth, res.Age);
            }

            Console.WriteLine();
            Console.WriteLine("6. Mostrar o valor médio das avaliações dos filmes de um determinado diretor");

            String diretor = "Steven Spielberg";
            var query6 = (from p in _db.Movies
                          where p.Director == diretor
                          select p.Rating).Average();
            
                Console.WriteLine("\t {0} - {1}", diretor, query6);
           

            Console.WriteLine();
            Console.WriteLine("7. Qual o elenco do filme melhor avaliado ?");

            var bestEvaluate = _db.Movies.Max(r => r.Rating);
           // var query7 = (from p in _db.Characters
          //                 group p by p.Movie.Rating into ratings
         //                  orderby ratings.Max(r => r.Movie.Rating) descending
        //                   select ratings.Key).First();
       //???????????????
      //                 Console.WriteLine("\t {0}",query7);

            Console.WriteLine();
            Console.WriteLine("8. Qual o elenco do filme com o maior faturamento?");

            var query8 = from p in _db.Characters
                         where p.Movie.Rating == bestEvaluate
                         select new
                         {
                            Name = p.Actor.Name
                         };
            //Também não funciona
            foreach (var res in query8)
            {
                Console.WriteLine("\t {0}", res.Name);
            }

            Console.WriteLine();
            Console.WriteLine("9. Gerar um relatório de aniversariantes, agrupando os atores pelo mês de aniverário.");

            Console.WriteLine("- - -   feito!  - - - ");
            Console.WriteLine();
        }

        static void Main_presencial(String[] args)
        {
            // acesso ao EF serah realizado pela variavel _db
            // essa dependencia da camada de apresentacao com
            // a camada de persistencia eh errada!
            MovieContext _db = new MovieContext();

            #region # LINQ - consultas

            Console.WriteLine();
            Console.WriteLine("1. Todos os filmes de acao");

            Console.WriteLine("1a. Modelo tradicional");
            List<Movie> filmes1a = new List<Movie>();
            foreach (Movie f in _db.Movies.Include("Genre"))
            {
                if (f.Genre.Name == "Action")
                    filmes1a.Add(f);
            }

            foreach (Movie filme in filmes1a)
            {
                Console.WriteLine("\t{0} - {1}", filme.Title, filme.ReleaseDate.Year);
            }

            Console.WriteLine("\n1b. Usando linq - query syntax");
            var filmes1b = from f in _db.Movies
                          where f.Genre.Name == "Action"
                          select f;
            foreach (Movie filme in filmes1b)
            {
                Console.WriteLine("\t{0} - {1}", filme.Title, filme.Director);
            }

            Console.WriteLine("\n1c. Usando linq - method syntax");
            var filmes1c = _db.Movies.Where(m => m.Genre.Name == "Action");
            foreach (Movie filme in filmes1c)
            {
                Console.WriteLine("\t{0}", filme.Title);
            }

 
            Console.WriteLine();
            Console.WriteLine("2. Todos os diretores de filmes do genero 'Action', com projecao");
            var filmes2 = from f in _db.Movies
                          where f.Genre.Name == "Action"
                          select f.Director;

            foreach (var nome in filmes2)
            {
                Console.WriteLine("\t{0}", nome);
            }

            Console.WriteLine();
            Console.WriteLine("3a. Todos os filmes de cada genero (query syntax):");
            var generosFilmes3a = from g in _db.Genres.Include(gen => gen.Movies)
                                select g;
            foreach (var gf in generosFilmes3a)
            {
                if (gf.Movies.Count > 0)
                {
                    Console.WriteLine("\nFilmes do genero: " + gf.Name);
                    foreach (var f in gf.Movies)
                    {
                        Console.WriteLine("\t{0} - {1}", f.Title, f.Rating);
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("3b. Todos os filmes de cada genero (method syntax):");

            var generosFilmes3b = _db.Genres.Include(gen => gen.Movies).ToList();

            foreach (Genre gf in generosFilmes3a)
            {
                if (gf.Movies.Count > 0)
                {
                    Console.WriteLine("\nFilmes do genero: " + gf.Name);
                    foreach (var f in gf.Movies)
                    {
                        Console.WriteLine("\t{0}", f.Title);
                    }
                }
            }


            Console.WriteLine();
            Console.WriteLine("4. Titulo e ano dos filmes do diretor Quentin Tarantino, com projcao em uma class anonima:");
            var tarantino = from f in _db.Movies
                            where f.Director == "Quentin Tarantino"
                             select new
                             {
                                 Ano = f.ReleaseDate.Year,
                                 f.Title
                             };

            foreach (var item in tarantino)
            {
                Console.WriteLine("{0} - {1}", item.Ano, item.Title);
            }

            Console.WriteLine();
            Console.WriteLine("5. Todos os gêneros ordenados pelo nome:");
            var q5 = _db.Genres.OrderByDescending(g => g.Name);
            foreach (var genero in q5)
            {
                Console.WriteLine("{0, 20}\t {1}", genero.Name, genero.Description.Substring(0, 30));
            }

            Console.WriteLine();
            Console.WriteLine("6. Numero de filmes agrupados pelo anos de lançamento:");
            var q6 = from f in _db.Movies
                     group f by f.ReleaseDate.Year into grupo
                     select new
                     {
                         Chave = grupo.Key,
                         NroFilmes = grupo.Count()
                     };

            foreach (var ano in q6.OrderByDescending(g => g.NroFilmes))
            {
                Console.WriteLine("Ano: {0}  Numero de filmes: {1}", ano.Chave, ano.NroFilmes);

            }

            Console.WriteLine();
            Console.WriteLine("7. Projeção do faturamento total, quantidade de filmes e avaliação média agrupadas por gênero:");
            var q7 = from f in _db.Movies
                     group f by f.Genre.Name into grpGen
                     select new
                     {
                         Categoria = grpGen.Key,
                         Faturamento = grpGen.Sum(e => e.Gross),
                         Avaliacao = grpGen.Average(e => e.Rating),
                         Quantidade = grpGen.Count()
                     };

            foreach (var genero in q7)
            {
                Console.WriteLine("Genero: {0}", genero.Categoria);
                Console.WriteLine("\tFaturamento total: {0}\n\t Avaliação média: {1}\n\tNumero de filmes: {2}",
                                genero.Faturamento, genero.Avaliacao, genero.Quantidade);
            }
            #endregion



        }

        static void Main_CRUd(string[] args)
        {
            Console.WriteLine("Hello World!");

            MovieContext _context = new MovieContext();

            Genre g1 = new Genre()
            {
                Name = "Comedia",
                Description = "Filmes de comedia"
            };

            Genre g2 = new Genre()
            {
                Name = "Ficcao",
                Description = "Filmes de ficcao"
            };

            _context.Genres.Add(g1);
            _context.Genres.Add(g2);

            _context.SaveChanges();

            List<Genre> genres = _context.Genres.ToList();

            foreach (Genre g in genres)
            {
                Console.WriteLine(String.Format("{0,2} {1,-10} {2}",
                                    g.GenreId, g.Name, g.Description));
            }

        }
    }
}
