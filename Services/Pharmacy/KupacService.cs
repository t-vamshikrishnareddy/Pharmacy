﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Core;
using Core.Entities;
using Core.Services;
using Data;
using NHibernate.Linq;

namespace Services
{
    public class KupacService : Service<Kupac>, IKupacService
    {
        public KupacService(NewDataLayer dataLayer) : base(dataLayer)
        {
        }

        public override void Update(Kupac entity)
        {
            using (var session = DataLayer.GetSession())
            {
                try
                {
                    var recepts = new List<Recept>();

                    if (entity.ReceptList != null)
                        recepts.AddRange(entity.ReceptList.Select(x => session.Get<Recept>(x.Id)));

                    entity.ReceptList = recepts;

                    session.SaveOrUpdate(entity);
                    session.Flush();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public override void Update(int id, Kupac entity)
        {
            using (var session = DataLayer.GetSession())
            {
                try
                {
                    var obj = Get(id);

                    obj.ReceptList  = entity.ReceptList;
                    obj.Pazar = entity.Pazar;
                    obj.Ime = entity.Ime;
                    obj.Kontakt = entity.Kontakt;

                    session.Update(obj);
                    session.BeginTransaction().Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public override void Create(Kupac entity)
        {
            using (var session = DataLayer.GetSession())
            {
                try
                {
                    var recepts = new List<Recept>();

                    if (entity.ReceptList != null)
                        recepts.AddRange(entity.ReceptList.Select(x => session.Get<Recept>(x.Id)));

                    entity.ReceptList = recepts;

                    session.SaveOrUpdate(entity);
                    session.BeginTransaction().Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public override Kupac Get(int id)
        {
            using (var session = DataLayer.GetSession())
            {
                var obj = session.Get<Kupac>(id);

                if (obj == null) return null;

                obj.ReceptList = session.Query<Recept>().Where(x => x.Deleted == false && x.Kupac.Id == obj.Id).ToList();
                return obj;
            }
        }

        public override void Delete(int id)
        {
            using (var session = DataLayer.GetSession())
            {
                try
                {
                    var entity = session.Get<Kupac>(id);

                    entity.Deleted = true;
                    session.Update(entity);
                    session.BeginTransaction().Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public DataTable GetDataTable()
        {
            var dataTable = new DataTable("Kupac");

            dataTable.Columns.Add("Id");
            dataTable.Columns.Add("LIme");
            dataTable.Columns.Add("Prezime");
            dataTable.Columns.Add("Email");
            dataTable.Columns.Add("BrojTelefona");

            dataTable.Columns.Add(Constants.ConcatenatedField, typeof (string), "Id + ' : ' +LIme");

            List<Kupac> objList;
            using (var session = DataLayer.GetSession())
                objList = session.QueryOver<Kupac>().Where(x => x.Deleted == false)?.List<Kupac>() as List<Kupac>;

            if (objList == null) return dataTable;
            objList.ForEach(
                x => dataTable.Rows.Add(x.Id, x.Ime.LIme, x.Ime.Prezime, x.Kontakt?.Email, x.Kontakt?.BrojTelefona));

            return dataTable;
        }
    }
}