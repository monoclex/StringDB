- [Classes](#classes)
	- [StringDB.Consts](#stringdbconsts)
	- [StringDB.Database](#stringdbdatabase)
	- [StringDB.IDatabase](#stringdbidatabase)
	- [StringDB.ITypeHandler](#stringdbitypehandler)
	- [StringDB.Reader.IPart](#stringdbreaderipart)
	- [StringDB.Reader.IReader](#stringdbreaderireader)
	- [StringDB.Reader.IReaderPair](#stringdbreaderireaderpair)
	- [StringDB.Reader.Reader](#stringdbreaderreader)
	- [StringDB.Reader.ReaderEnumerator](#stringdbreaderreaderenumerator)
	- [StringDB.Reader.ReaderPair](#stringdbreaderreaderpair)
	- [StringDB.Reader.ThreadSafeReaderPair](#stringdbreaderthreadsafereaderpair)
	- [StringDB.ThreadSafeDatabase](#stringdbthreadsafedatabase)
	- [StringDB.TypeHandler&lt;TClass1&gt;](#stringdbtypehandlertclass1)
	- [StringDB.TypeHandlerDoesntExist](#stringdbtypehandlerdoesntexist)
	- [StringDB.TypeHandlerExists](#stringdbtypehandlerexists)
	- [StringDB.TypeManager](#stringdbtypemanager)
	- [StringDB.Writer.IWriter](#stringdbwriteriwriter)
	- [StringDB.Writer.Writer](#stringdbwriterwriter)

---

- [Methods](#methods)
	- [StringDB.Database.CleanFrom(StringDB.IDatabase dbCleanFrom)](#stringdbdatabasecleanfromstringdbidatabase-dbcleanfrom)
	- [StringDB.Database.CleanTo(StringDB.IDatabase dbCleanTo)](#stringdbdatabasecleantostringdbidatabase-dbcleanto)
	- [StringDB.Database.Dispose](#stringdbdatabasedispose)
	- [StringDB.Database.DrainBuffer](#stringdbdatabasedrainbuffer)
	- [StringDB.Database.First](#stringdbdatabasefirst)
	- [StringDB.Database.Flush](#stringdbdatabaseflush)
	- [StringDB.Database.FromFile(System.String name)](#stringdbdatabasefromfilesystemstring-name)
	- [StringDB.Database.FromStream(System.IO.Stream s, System.Boolean disposeStream)](#stringdbdatabasefromstreamsystemiostream-s-systemboolean-disposestream)
	- [StringDB.Database.Get&lt;TParam1&gt;(&lt;TParam1&gt;)](#stringdbdatabasegettparam1tparam1)
	- [StringDB.Database.Get&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;)](#stringdbdatabasegettparam1stringdbtypehandlertparam1-tparam1)
	- [StringDB.Database.GetAll&lt;TParam1&gt;(&lt;TParam1&gt;)](#stringdbdatabasegetalltparam1tparam1)
	- [StringDB.Database.GetAll&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;)](#stringdbdatabasegetalltparam1stringdbtypehandlertparam1-tparam1)
	- [StringDB.Database.GetEnumerator](#stringdbdatabasegetenumerator)
	- [StringDB.Database.Insert&lt;TParam1, TParam2&gt;(&lt;TParam1&gt;, &lt;TParam2&gt;)](#stringdbdatabaseinserttparam1-tparam2tparam1-tparam2)
	- [StringDB.Database.Insert&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, &lt;TParam1&gt;, &lt;TParam2&gt;)](#stringdbdatabaseinserttparam1-tparam2stringdbtypehandlertparam1-stringdbtypehandlertparam2-tparam1-tparam2)
	- [StringDB.Database.Insert&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;})](#stringdbdatabaseinserttparam1-tparam2stringdbtypehandlertparam1-stringdbtypehandlertparam2-systemcollectionsgenerickeyvaluepairtparam1-tparam2)
	- [StringDB.Database.Insert&lt;TParam1, TParam2&gt;(System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;})](#stringdbdatabaseinserttparam1-tparam2systemcollectionsgenerickeyvaluepairtparam1-tparam2)
	- [StringDB.Database.InsertRange&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;}})](#stringdbdatabaseinsertrangetparam1-tparam2stringdbtypehandlertparam1-stringdbtypehandlertparam2-systemcollectionsgenericienumerablesystemcollectionsgenerickeyvaluepairtparam1-tparam2)
	- [StringDB.Database.InsertRange&lt;TParam1, TParam2&gt;(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;}})](#stringdbdatabaseinsertrangetparam1-tparam2systemcollectionsgenericienumerablesystemcollectionsgenerickeyvaluepairtparam1-tparam2)
	- [StringDB.Database.MakeThreadSafe](#stringdbdatabasemakethreadsafe)
	- [StringDB.Database.OverwriteValue&lt;TParam1&gt;(StringDB.Reader.IReaderPair, &lt;TParam1&gt;)](#stringdbdatabaseoverwritevaluetparam1stringdbreaderireaderpair-tparam1)
	- [StringDB.Database.OverwriteValue&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.Reader.IReaderPair, &lt;TParam1&gt;)](#stringdbdatabaseoverwritevaluetparam1stringdbtypehandlertparam1-stringdbreaderireaderpair-tparam1)
	- [StringDB.Database.System#Collections#IEnumerable#GetEnumerator](#stringdbdatabasesystem#collections#ienumerable#getenumerator)
	- [StringDB.Database.TryGet&lt;TParam1&gt;(&lt;TParam1&gt;, StringDB.Reader.IReaderPair@)](#stringdbdatabasetrygettparam1tparam1-stringdbreaderireaderpair@)
	- [StringDB.Database.TryGet&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;, StringDB.Reader.IReaderPair@)](#stringdbdatabasetrygettparam1stringdbtypehandlertparam1-tparam1-stringdbreaderireaderpair@)
	- [StringDB.IDatabase.CleanFrom(StringDB.IDatabase dbCleanFrom)](#stringdbidatabasecleanfromstringdbidatabase-dbcleanfrom)
	- [StringDB.IDatabase.CleanTo(StringDB.IDatabase dbCleanTo)](#stringdbidatabasecleantostringdbidatabase-dbcleanto)
	- [StringDB.IDatabase.Flush](#stringdbidatabaseflush)
	- [StringDB.Reader.IReader.DrainBuffer](#stringdbreaderireaderdrainbuffer)
	- [StringDB.Reader.IReader.First](#stringdbreaderireaderfirst)
	- [StringDB.Reader.IReader.Get&lt;TParam1&gt;(&lt;TParam1&gt;)](#stringdbreaderireadergettparam1tparam1)
	- [StringDB.Reader.IReader.Get&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;)](#stringdbreaderireadergettparam1stringdbtypehandlertparam1-tparam1)
	- [StringDB.Reader.IReader.GetAll&lt;TParam1&gt;(&lt;TParam1&gt;)](#stringdbreaderireadergetalltparam1tparam1)
	- [StringDB.Reader.IReader.GetAll&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;)](#stringdbreaderireadergetalltparam1stringdbtypehandlertparam1-tparam1)
	- [StringDB.Reader.IReader.TryGet&lt;TParam1&gt;(&lt;TParam1&gt;, StringDB.Reader.IReaderPair@)](#stringdbreaderireadertrygettparam1tparam1-stringdbreaderireaderpair@)
	- [StringDB.Reader.IReader.TryGet&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;, StringDB.Reader.IReaderPair@)](#stringdbreaderireadertrygettparam1stringdbtypehandlertparam1-tparam1-stringdbreaderireaderpair@)
	- [StringDB.Reader.IReaderPair.GetIndexAs&lt;TParam1&gt;](#stringdbreaderireaderpairgetindexastparam1)
	- [StringDB.Reader.IReaderPair.GetValue&lt;TParam1&gt;](#stringdbreaderireaderpairgetvaluetparam1)
	- [StringDB.Reader.IReaderPair.GetValueAs&lt;TParam1&gt;](#stringdbreaderireaderpairgetvalueastparam1)
	- [StringDB.Reader.Reader.DrainBuffer](#stringdbreaderreaderdrainbuffer)
	- [StringDB.Reader.Reader.First](#stringdbreaderreaderfirst)
	- [StringDB.Reader.Reader.Get&lt;TParam1&gt;(&lt;TParam1&gt;)](#stringdbreaderreadergettparam1tparam1)
	- [StringDB.Reader.Reader.Get&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;)](#stringdbreaderreadergettparam1stringdbtypehandlertparam1-tparam1)
	- [StringDB.Reader.Reader.GetAll&lt;TParam1&gt;(&lt;TParam1&gt;)](#stringdbreaderreadergetalltparam1tparam1)
	- [StringDB.Reader.Reader.GetAll&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;)](#stringdbreaderreadergetalltparam1stringdbtypehandlertparam1-tparam1)
	- [StringDB.Reader.Reader.GetEnumerator](#stringdbreaderreadergetenumerator)
	- [StringDB.Reader.Reader.System#Collections#IEnumerable#GetEnumerator](#stringdbreaderreadersystem#collections#ienumerable#getenumerator)
	- [StringDB.Reader.Reader.TryGet&lt;TParam1&gt;(&lt;TParam1&gt;, StringDB.Reader.IReaderPair@)](#stringdbreaderreadertrygettparam1tparam1-stringdbreaderireaderpair@)
	- [StringDB.Reader.Reader.TryGet&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;, StringDB.Reader.IReaderPair@)](#stringdbreaderreadertrygettparam1stringdbtypehandlertparam1-tparam1-stringdbreaderireaderpair@)
	- [StringDB.Reader.ReaderEnumerator.Dispose](#stringdbreaderreaderenumeratordispose)
	- [StringDB.Reader.ReaderEnumerator.MoveNext](#stringdbreaderreaderenumeratormovenext)
	- [StringDB.Reader.ReaderEnumerator.Reset](#stringdbreaderreaderenumeratorreset)
	- [StringDB.Reader.ReaderPair.GetIndexAs&lt;TParam1&gt;](#stringdbreaderreaderpairgetindexastparam1)
	- [StringDB.Reader.ReaderPair.GetValue&lt;TParam1&gt;](#stringdbreaderreaderpairgetvaluetparam1)
	- [StringDB.Reader.ReaderPair.GetValueAs&lt;TParam1&gt;](#stringdbreaderreaderpairgetvalueastparam1)
	- [StringDB.Reader.ReaderPair.ToString](#stringdbreaderreaderpairtostring)
	- [StringDB.Reader.ThreadSafeReaderPair.FromPair(StringDB.Reader.IReaderPair readerPair, System.Object lock)](#stringdbreaderthreadsafereaderpairfrompairstringdbreaderireaderpair-readerpair-systemobject-lock)
	- [StringDB.Reader.ThreadSafeReaderPair.GetIndexAs&lt;TParam1&gt;](#stringdbreaderthreadsafereaderpairgetindexastparam1)
	- [StringDB.Reader.ThreadSafeReaderPair.GetValue&lt;TParam1&gt;](#stringdbreaderthreadsafereaderpairgetvaluetparam1)
	- [StringDB.Reader.ThreadSafeReaderPair.GetValueAs&lt;TParam1&gt;](#stringdbreaderthreadsafereaderpairgetvalueastparam1)
	- [StringDB.Reader.ThreadSafeReaderPair.ToString](#stringdbreaderthreadsafereaderpairtostring)
	- [StringDB.ThreadSafeDatabase.CleanFrom(StringDB.IDatabase dbCleanFrom)](#stringdbthreadsafedatabasecleanfromstringdbidatabase-dbcleanfrom)
	- [StringDB.ThreadSafeDatabase.CleanTo(StringDB.IDatabase dbCleanTo)](#stringdbthreadsafedatabasecleantostringdbidatabase-dbcleanto)
	- [StringDB.ThreadSafeDatabase.Dispose](#stringdbthreadsafedatabasedispose)
	- [StringDB.ThreadSafeDatabase.DrainBuffer](#stringdbthreadsafedatabasedrainbuffer)
	- [StringDB.ThreadSafeDatabase.First](#stringdbthreadsafedatabasefirst)
	- [StringDB.ThreadSafeDatabase.Flush](#stringdbthreadsafedatabaseflush)
	- [StringDB.ThreadSafeDatabase.FromDatabase(StringDB.IDatabase db)](#stringdbthreadsafedatabasefromdatabasestringdbidatabase-db)
	- [StringDB.ThreadSafeDatabase.Get&lt;TParam1&gt;(&lt;TParam1&gt;)](#stringdbthreadsafedatabasegettparam1tparam1)
	- [StringDB.ThreadSafeDatabase.Get&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;)](#stringdbthreadsafedatabasegettparam1stringdbtypehandlertparam1-tparam1)
	- [StringDB.ThreadSafeDatabase.GetAll&lt;TParam1&gt;(&lt;TParam1&gt;)](#stringdbthreadsafedatabasegetalltparam1tparam1)
	- [StringDB.ThreadSafeDatabase.GetAll&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;)](#stringdbthreadsafedatabasegetalltparam1stringdbtypehandlertparam1-tparam1)
	- [StringDB.ThreadSafeDatabase.GetEnumerator](#stringdbthreadsafedatabasegetenumerator)
	- [StringDB.ThreadSafeDatabase.Insert&lt;TParam1, TParam2&gt;(&lt;TParam1&gt;, &lt;TParam2&gt;)](#stringdbthreadsafedatabaseinserttparam1-tparam2tparam1-tparam2)
	- [StringDB.ThreadSafeDatabase.Insert&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, &lt;TParam1&gt;, &lt;TParam2&gt;)](#stringdbthreadsafedatabaseinserttparam1-tparam2stringdbtypehandlertparam1-stringdbtypehandlertparam2-tparam1-tparam2)
	- [StringDB.ThreadSafeDatabase.Insert&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;})](#stringdbthreadsafedatabaseinserttparam1-tparam2stringdbtypehandlertparam1-stringdbtypehandlertparam2-systemcollectionsgenerickeyvaluepairtparam1-tparam2)
	- [StringDB.ThreadSafeDatabase.Insert&lt;TParam1, TParam2&gt;(System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;})](#stringdbthreadsafedatabaseinserttparam1-tparam2systemcollectionsgenerickeyvaluepairtparam1-tparam2)
	- [StringDB.ThreadSafeDatabase.InsertRange&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;}})](#stringdbthreadsafedatabaseinsertrangetparam1-tparam2stringdbtypehandlertparam1-stringdbtypehandlertparam2-systemcollectionsgenericienumerablesystemcollectionsgenerickeyvaluepairtparam1-tparam2)
	- [StringDB.ThreadSafeDatabase.InsertRange&lt;TParam1, TParam2&gt;(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;}})](#stringdbthreadsafedatabaseinsertrangetparam1-tparam2systemcollectionsgenericienumerablesystemcollectionsgenerickeyvaluepairtparam1-tparam2)
	- [StringDB.ThreadSafeDatabase.OverwriteValue&lt;TParam1&gt;(StringDB.Reader.IReaderPair, &lt;TParam1&gt;)](#stringdbthreadsafedatabaseoverwritevaluetparam1stringdbreaderireaderpair-tparam1)
	- [StringDB.ThreadSafeDatabase.OverwriteValue&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.Reader.IReaderPair, &lt;TParam1&gt;)](#stringdbthreadsafedatabaseoverwritevaluetparam1stringdbtypehandlertparam1-stringdbreaderireaderpair-tparam1)
	- [StringDB.ThreadSafeDatabase.System#Collections#IEnumerable#GetEnumerator](#stringdbthreadsafedatabasesystem#collections#ienumerable#getenumerator)
	- [StringDB.ThreadSafeDatabase.TryGet&lt;TParam1&gt;(&lt;TParam1&gt;, StringDB.Reader.IReaderPair@)](#stringdbthreadsafedatabasetrygettparam1tparam1-stringdbreaderireaderpair@)
	- [StringDB.ThreadSafeDatabase.TryGet&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;, StringDB.Reader.IReaderPair@)](#stringdbthreadsafedatabasetrygettparam1stringdbtypehandlertparam1-tparam1-stringdbreaderireaderpair@)
	- [StringDB.TypeHandler&lt;TClass1&gt;.Compare(&lt;TClass1&gt; item1, &lt;TClass1&gt; item2)](#stringdbtypehandlertclass1comparetclass1-item1-tclass1-item2)
	- [StringDB.TypeHandler&lt;TClass1&gt;.GetLength(&lt;TClass1&gt; item)](#stringdbtypehandlertclass1getlengthtclass1-item)
	- [StringDB.TypeHandler&lt;TClass1&gt;.Read(System.IO.BinaryReader br, System.Int64 len)](#stringdbtypehandlertclass1readsystemiobinaryreader-br-systemint64-len)
	- [StringDB.TypeHandler&lt;TClass1&gt;.Write(System.IO.BinaryWriter bw, &lt;TClass1&gt; item)](#stringdbtypehandlertclass1writesystemiobinarywriter-bw-tclass1-item)
	- [StringDB.TypeManager.GetHandlerFor(System.Byte id)](#stringdbtypemanagergethandlerforsystembyte-id)
	- [StringDB.TypeManager.GetHandlerFor&lt;TParam1&gt;](#stringdbtypemanagergethandlerfortparam1)
	- [StringDB.TypeManager.OverridingRegisterType&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;} t)](#stringdbtypemanageroverridingregistertypetparam1stringdbtypehandlertparam1-t)
	- [StringDB.TypeManager.RegisterType&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;} t)](#stringdbtypemanagerregistertypetparam1stringdbtypehandlertparam1-t)
	- [StringDB.Writer.IWriter.Flush](#stringdbwriteriwriterflush)
	- [StringDB.Writer.IWriter.Insert&lt;TParam1, TParam2&gt;(&lt;TParam1&gt;, &lt;TParam2&gt;)](#stringdbwriteriwriterinserttparam1-tparam2tparam1-tparam2)
	- [StringDB.Writer.IWriter.Insert&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, &lt;TParam1&gt;, &lt;TParam2&gt;)](#stringdbwriteriwriterinserttparam1-tparam2stringdbtypehandlertparam1-stringdbtypehandlertparam2-tparam1-tparam2)
	- [StringDB.Writer.IWriter.Insert&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;})](#stringdbwriteriwriterinserttparam1-tparam2stringdbtypehandlertparam1-stringdbtypehandlertparam2-systemcollectionsgenerickeyvaluepairtparam1-tparam2)
	- [StringDB.Writer.IWriter.Insert&lt;TParam1, TParam2&gt;(System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;})](#stringdbwriteriwriterinserttparam1-tparam2systemcollectionsgenerickeyvaluepairtparam1-tparam2)
	- [StringDB.Writer.IWriter.InsertRange&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;}})](#stringdbwriteriwriterinsertrangetparam1-tparam2stringdbtypehandlertparam1-stringdbtypehandlertparam2-systemcollectionsgenericienumerablesystemcollectionsgenerickeyvaluepairtparam1-tparam2)
	- [StringDB.Writer.IWriter.InsertRange&lt;TParam1, TParam2&gt;(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;}})](#stringdbwriteriwriterinsertrangetparam1-tparam2systemcollectionsgenericienumerablesystemcollectionsgenerickeyvaluepairtparam1-tparam2)
	- [StringDB.Writer.IWriter.OverwriteValue&lt;TParam1&gt;(StringDB.Reader.IReaderPair, &lt;TParam1&gt;)](#stringdbwriteriwriteroverwritevaluetparam1stringdbreaderireaderpair-tparam1)
	- [StringDB.Writer.IWriter.OverwriteValue&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.Reader.IReaderPair, &lt;TParam1&gt;)](#stringdbwriteriwriteroverwritevaluetparam1stringdbtypehandlertparam1-stringdbreaderireaderpair-tparam1)
	- [StringDB.Writer.Writer.#ctor(System.IO.Stream s)](#stringdbwriterwriter#ctorsystemiostream-s)
	- [StringDB.Writer.Writer.Flush](#stringdbwriterwriterflush)
	- [StringDB.Writer.Writer.Insert&lt;TParam1, TParam2&gt;(&lt;TParam1&gt;, &lt;TParam2&gt;)](#stringdbwriterwriterinserttparam1-tparam2tparam1-tparam2)
	- [StringDB.Writer.Writer.Insert&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, &lt;TParam1&gt;, &lt;TParam2&gt;)](#stringdbwriterwriterinserttparam1-tparam2stringdbtypehandlertparam1-stringdbtypehandlertparam2-tparam1-tparam2)
	- [StringDB.Writer.Writer.Insert&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;})](#stringdbwriterwriterinserttparam1-tparam2stringdbtypehandlertparam1-stringdbtypehandlertparam2-systemcollectionsgenerickeyvaluepairtparam1-tparam2)
	- [StringDB.Writer.Writer.Insert&lt;TParam1, TParam2&gt;(System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;})](#stringdbwriterwriterinserttparam1-tparam2systemcollectionsgenerickeyvaluepairtparam1-tparam2)
	- [StringDB.Writer.Writer.InsertRange&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;}})](#stringdbwriterwriterinsertrangetparam1-tparam2stringdbtypehandlertparam1-stringdbtypehandlertparam2-systemcollectionsgenericienumerablesystemcollectionsgenerickeyvaluepairtparam1-tparam2)
	- [StringDB.Writer.Writer.InsertRange&lt;TParam1, TParam2&gt;(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;}})](#stringdbwriterwriterinsertrangetparam1-tparam2systemcollectionsgenericienumerablesystemcollectionsgenerickeyvaluepairtparam1-tparam2)
	- [StringDB.Writer.Writer.OverwriteValue&lt;TParam1&gt;(StringDB.Reader.IReaderPair, &lt;TParam1&gt;)](#stringdbwriterwriteroverwritevaluetparam1stringdbreaderireaderpair-tparam1)
	- [StringDB.Writer.Writer.OverwriteValue&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.Reader.IReaderPair, &lt;TParam1&gt;)](#stringdbwriterwriteroverwritevaluetparam1stringdbtypehandlertparam1-stringdbreaderireaderpair-tparam1)

---

- [Properties](#properties)
	- [StringDB.ITypeHandler.Id](#stringdbitypehandlerid)
	- [StringDB.ITypeHandler.Type](#stringdbitypehandlertype)
	- [StringDB.Reader.IPart.InitialByte](#stringdbreaderipartinitialbyte)
	- [StringDB.Reader.IPart.NextPart](#stringdbreaderipartnextpart)
	- [StringDB.Reader.IPart.Position](#stringdbreaderipartposition)
	- [StringDB.Reader.IReaderPair.ByteArrayIndex](#stringdbreaderireaderpairbytearrayindex)
	- [StringDB.Reader.IReaderPair.DataPosition](#stringdbreaderireaderpairdataposition)
	- [StringDB.Reader.IReaderPair.Index](#stringdbreaderireaderpairindex)
	- [StringDB.Reader.IReaderPair.Position](#stringdbreaderireaderpairposition)
	- [StringDB.Reader.IReaderPair.ValueLength](#stringdbreaderireaderpairvaluelength)
	- [StringDB.Reader.ReaderEnumerator.Current](#stringdbreaderreaderenumeratorcurrent)
	- [StringDB.Reader.ReaderPair.ByteArrayIndex](#stringdbreaderreaderpairbytearrayindex)
	- [StringDB.Reader.ReaderPair.DataPosition](#stringdbreaderreaderpairdataposition)
	- [StringDB.Reader.ReaderPair.Index](#stringdbreaderreaderpairindex)
	- [StringDB.Reader.ReaderPair.Position](#stringdbreaderreaderpairposition)
	- [StringDB.Reader.ReaderPair.ValueLength](#stringdbreaderreaderpairvaluelength)
	- [StringDB.Reader.ThreadSafeReaderPair.ByteArrayIndex](#stringdbreaderthreadsafereaderpairbytearrayindex)
	- [StringDB.Reader.ThreadSafeReaderPair.DataPosition](#stringdbreaderthreadsafereaderpairdataposition)
	- [StringDB.Reader.ThreadSafeReaderPair.Index](#stringdbreaderthreadsafereaderpairindex)
	- [StringDB.Reader.ThreadSafeReaderPair.Position](#stringdbreaderthreadsafereaderpairposition)
	- [StringDB.Reader.ThreadSafeReaderPair.ValueLength](#stringdbreaderthreadsafereaderpairvaluelength)
	- [StringDB.TypeHandler&lt;TClass1&gt;.Id](#stringdbtypehandlertclass1id)
	- [StringDB.TypeHandler&lt;TClass1&gt;.Type](#stringdbtypehandlertclass1type)

---

- [Fields](#fields)
	- [StringDB.Consts.DeletedValue](#stringdbconstsdeletedvalue)
	- [StringDB.Consts.IndexSeperator](#stringdbconstsindexseperator)
	- [StringDB.Consts.IsByteValue](#stringdbconstsisbytevalue)
	- [StringDB.Consts.IsLongValue](#stringdbconstsislongvalue)
	- [StringDB.Consts.IsUIntValue](#stringdbconstsisuintvalue)
	- [StringDB.Consts.IsUShortValue](#stringdbconstsisushortvalue)
	- [StringDB.Consts.MaxLength](#stringdbconstsmaxlength)
	- [StringDB.Consts.NoIndex](#stringdbconstsnoindex)

---

## Classes

StringDB.Consts
---
StringDB.Database
---
#### Summary
Some kind of wwriter that writes stuff.

StringDB.IDatabase
---
#### Summary
A StringDB database, used to encapsulate an IReader and an IWriter together for easy usage.

StringDB.ITypeHandler
---
#### Summary
A generic interface for a given TypeHandler

StringDB.Reader.IPart
---
StringDB.Reader.IReader
---
#### Summary
Defines a reader. Use it to read out data

StringDB.Reader.IReaderPair
---
#### Summary
A pair of data - this correlates an index to it's corresponding value.

StringDB.Reader.Reader
---
#### Summary
A Reader that reads out a StringDB database file.

StringDB.Reader.ReaderEnumerator
---
#### Summary
Used to enumerate over a StringDB.

StringDB.Reader.ReaderPair
---
#### Summary
A pair of data - this correlates an index to it's corresponding value.

StringDB.Reader.ThreadSafeReaderPair
---
#### Summary
Make a ReaderPair thread safe.

StringDB.ThreadSafeDatabase
---
#### Summary
Uses a lock before doing any action to add thread safety

StringDB.TypeHandler&lt;TClass1&gt;
---
#### Summary
Allows StringDB to handle a new type, without much effort.

Param Name | Summary
---------- | -------
T | The type

StringDB.TypeHandlerDoesntExist
---
#### Summary
An exception that gets thrown when attempting to get a Type that doesn't exist.

StringDB.TypeHandlerExists
---
#### Summary
An exception that gets thrown when attempting to register a Type if it already exists

StringDB.TypeManager
---
#### Summary
Manages the types StringDB can read and write. Add your own if you need more types!

StringDB.Writer.IWriter
---
#### Summary
Some kind of wwriter that writes stuff.

StringDB.Writer.Writer
---
#### Summary
Some kind of wwriter that writes stuff.

## Methods

StringDB.Database.CleanFrom(StringDB.IDatabase dbCleanFrom)
---
#### Summary
Cleans out the database specified, and copies all of the contents of the other database into this one. You may be able to experience a smaller DB file if you've used StringDB to not to perfectionist values.

Param Name | Summary
---------- | -------
dbCleanFrom | The database to clean up

StringDB.Database.CleanTo(StringDB.IDatabase dbCleanTo)
---
#### Summary
Cleans out the current database, and copies all of the contents of this database into the other one. You may be able to experience a smaller DB file if you've used StringDB to not to perfectionist values.

Param Name | Summary
---------- | -------
dbCleanTo | The database that will be used to insert the other database's values into

StringDB.Database.Dispose
---
StringDB.Database.DrainBuffer
---
#### Summary
Clears out the buffer. Will cause performance issues if you do it too often.

StringDB.Database.First
---
#### Summary
Gets the very first element in the database

StringDB.Database.Flush
---
#### Summary
Flushes the prepending data to write to the stream

StringDB.Database.FromFile(System.String name)
---
#### Summary
Create a new Database from a string name to open a file

Param Name | Summary
---------- | -------
name | The name of the file

StringDB.Database.FromStream(System.IO.Stream s, System.Boolean disposeStream)
---
#### Summary
Create a new Database from a stream

Param Name | Summary
---------- | -------
s | The stream to be using
disposeStream | If the stream should be disposed after we're done using it

StringDB.Database.Get&lt;TParam1&gt;(&lt;TParam1&gt;)
---
#### Summary
Gets the ReaderPair responsible for a given index

StringDB.Database.Get&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;)
---
#### Summary
Gets the ReaderPair responsible for a given index

StringDB.Database.GetAll&lt;TParam1&gt;(&lt;TParam1&gt;)
---
#### Summary
Gets the multiple ReaderPairs responsible for a given index

StringDB.Database.GetAll&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;)
---
#### Summary
Gets the multiple ReaderPairs responsible for a given index

StringDB.Database.GetEnumerator
---
StringDB.Database.Insert&lt;TParam1, TParam2&gt;(&lt;TParam1&gt;, &lt;TParam2&gt;)
---
#### Summary
Insert an item into the database

StringDB.Database.Insert&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, &lt;TParam1&gt;, &lt;TParam2&gt;)
---
#### Summary
Insert an item into the database

StringDB.Database.Insert&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;})
---
#### Summary
Insert an item into the database

StringDB.Database.Insert&lt;TParam1, TParam2&gt;(System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;})
---
#### Summary
Insert an item into the database

StringDB.Database.InsertRange&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;}})
---
#### Summary
Insert multiple items into the database.

StringDB.Database.InsertRange&lt;TParam1, TParam2&gt;(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;}})
---
#### Summary
Insert multiple items into the database.

StringDB.Database.MakeThreadSafe
---
#### Summary
Wraps this database into thread safety using a ThreadSafeDatabase

StringDB.Database.OverwriteValue&lt;TParam1&gt;(StringDB.Reader.IReaderPair, &lt;TParam1&gt;)
---
#### Summary
Overwrite a value. Note: You should call the database cleaning functions if you do this too frequently.

StringDB.Database.OverwriteValue&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.Reader.IReaderPair, &lt;TParam1&gt;)
---
#### Summary
Overwrite a value. Note: You should call the database cleaning functions if you do this too frequently.

StringDB.Database.System#Collections#IEnumerable#GetEnumerator
---
StringDB.Database.TryGet&lt;TParam1&gt;(&lt;TParam1&gt;, StringDB.Reader.IReaderPair@)
---
#### Summary
Attempts to get the ReaderPair

StringDB.Database.TryGet&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;, StringDB.Reader.IReaderPair@)
---
#### Summary
Attempts to get the ReaderPair

StringDB.IDatabase.CleanFrom(StringDB.IDatabase dbCleanFrom)
---
#### Summary
Cleans out the database specified, and copies all of the contents of the other database into this one. You may be able to experience a smaller DB file if you've used StringDB to not to perfectionist values.

Param Name | Summary
---------- | -------
dbCleanFrom | The database to clean up

StringDB.IDatabase.CleanTo(StringDB.IDatabase dbCleanTo)
---
#### Summary
Cleans out the current database, and copies all of the contents of this database into the other one. You may be able to experience a smaller DB file if you've used StringDB to not to perfectionist values.

Param Name | Summary
---------- | -------
dbCleanTo | The database that will be used to insert the other database's values into

StringDB.IDatabase.Flush
---
#### Summary
Clears all buffers for this stream and causes any buffered data to be written to the underlying device.

StringDB.Reader.IReader.DrainBuffer
---
#### Summary
Clears out the buffer. Will cause performance issues if you do it too often.

StringDB.Reader.IReader.First
---
#### Summary
Gets the very first element in the database

StringDB.Reader.IReader.Get&lt;TParam1&gt;(&lt;TParam1&gt;)
---
#### Summary
Gets the ReaderPair responsible for a given index

StringDB.Reader.IReader.Get&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;)
---
#### Summary
Gets the ReaderPair responsible for a given index

StringDB.Reader.IReader.GetAll&lt;TParam1&gt;(&lt;TParam1&gt;)
---
#### Summary
Gets the multiple ReaderPairs responsible for a given index

StringDB.Reader.IReader.GetAll&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;)
---
#### Summary
Gets the multiple ReaderPairs responsible for a given index

StringDB.Reader.IReader.TryGet&lt;TParam1&gt;(&lt;TParam1&gt;, StringDB.Reader.IReaderPair@)
---
#### Summary
Attempts to get the ReaderPair

StringDB.Reader.IReader.TryGet&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;, StringDB.Reader.IReaderPair@)
---
#### Summary
Attempts to get the ReaderPair

StringDB.Reader.IReaderPair.GetIndexAs&lt;TParam1&gt;
---
#### Summary
Get the index as any type the TypeHandler can handle.

Param Name | Summary
---------- | -------
T | The type to read the index as.

StringDB.Reader.IReaderPair.GetValue&lt;TParam1&gt;
---
#### Summary
Read the data stored at the index as the type it was meant to be.

#### Remarks
See GetValueAs to try convert the value into the specified type.

Param Name | Summary
---------- | -------
T | The type of the data that is stored.

StringDB.Reader.IReaderPair.GetValueAs&lt;TParam1&gt;
---
#### Summary
Read the data stored at the index and ignore the type it should be, and try to convert it.

Param Name | Summary
---------- | -------
T | The type you want it to be.

StringDB.Reader.Reader.DrainBuffer
---
#### Summary
Clears out the buffer. Will cause performance issues if you do it too often.

StringDB.Reader.Reader.First
---
#### Summary
Gets the very first element in the database

StringDB.Reader.Reader.Get&lt;TParam1&gt;(&lt;TParam1&gt;)
---
#### Summary
Gets the ReaderPair responsible for a given index

StringDB.Reader.Reader.Get&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;)
---
#### Summary
Gets the ReaderPair responsible for a given index

StringDB.Reader.Reader.GetAll&lt;TParam1&gt;(&lt;TParam1&gt;)
---
#### Summary
Gets the multiple ReaderPairs responsible for a given index

StringDB.Reader.Reader.GetAll&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;)
---
#### Summary
Gets the multiple ReaderPairs responsible for a given index

StringDB.Reader.Reader.GetEnumerator
---
StringDB.Reader.Reader.System#Collections#IEnumerable#GetEnumerator
---
StringDB.Reader.Reader.TryGet&lt;TParam1&gt;(&lt;TParam1&gt;, StringDB.Reader.IReaderPair@)
---
#### Summary
Attempts to get the ReaderPair

StringDB.Reader.Reader.TryGet&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;, StringDB.Reader.IReaderPair@)
---
#### Summary
Attempts to get the ReaderPair

StringDB.Reader.ReaderEnumerator.Dispose
---
#### Summary
Removes references to the raw reader and the part we're on.

StringDB.Reader.ReaderEnumerator.MoveNext
---
#### Summary
Retireves the next element in the sequence

StringDB.Reader.ReaderEnumerator.Reset
---
#### Summary
Resets it back to the start.

StringDB.Reader.ReaderPair.GetIndexAs&lt;TParam1&gt;
---
#### Summary
Get the index as any type the TypeHandler can handle.

Param Name | Summary
---------- | -------
T | The type to read the index as.

StringDB.Reader.ReaderPair.GetValue&lt;TParam1&gt;
---
#### Summary
Read the data stored at the index as the type it was meant to be.

#### Remarks
See GetValueAs to try convert the value into the specified type.

Param Name | Summary
---------- | -------
T | The type of the data that is stored.

StringDB.Reader.ReaderPair.GetValueAs&lt;TParam1&gt;
---
#### Summary
Read the data stored at the index and ignore the type it should be, and try to convert it.

Param Name | Summary
---------- | -------
T | The type you want it to be.

StringDB.Reader.ReaderPair.ToString
---
#### Summary
A simple string form of the item.

StringDB.Reader.ThreadSafeReaderPair.FromPair(StringDB.Reader.IReaderPair readerPair, System.Object lock)
---
#### Summary
Turn a given ReaderPair into a ThreadSafeReaderPair

Param Name | Summary
---------- | -------
readerPair | The ReaderPair
lock | The lock to lock onto

StringDB.Reader.ThreadSafeReaderPair.GetIndexAs&lt;TParam1&gt;
---
#### Summary
Get the index as any type the TypeHandler can handle.

Param Name | Summary
---------- | -------
T | The type to read the index as.

StringDB.Reader.ThreadSafeReaderPair.GetValue&lt;TParam1&gt;
---
#### Summary
Read the data stored at the index as the type it was meant to be.

#### Remarks
See GetValueAs to try convert the value into the specified type.

Param Name | Summary
---------- | -------
T | The type of the data that is stored.

StringDB.Reader.ThreadSafeReaderPair.GetValueAs&lt;TParam1&gt;
---
#### Summary
Read the data stored at the index and ignore the type it should be, and try to convert it.

Param Name | Summary
---------- | -------
T | The type you want it to be.

StringDB.Reader.ThreadSafeReaderPair.ToString
---
#### Summary
A simple string form of the item.

StringDB.ThreadSafeDatabase.CleanFrom(StringDB.IDatabase dbCleanFrom)
---
#### Summary
Cleans out the database specified, and copies all of the contents of the other database into this one. You may be able to experience a smaller DB file if you've used StringDB to not to perfectionist values.

Param Name | Summary
---------- | -------
dbCleanFrom | The database to clean up

StringDB.ThreadSafeDatabase.CleanTo(StringDB.IDatabase dbCleanTo)
---
#### Summary
Cleans out the current database, and copies all of the contents of this database into the other one. You may be able to experience a smaller DB file if you've used StringDB to not to perfectionist values.

Param Name | Summary
---------- | -------
dbCleanTo | The database that will be used to insert the other database's values into

StringDB.ThreadSafeDatabase.Dispose
---
StringDB.ThreadSafeDatabase.DrainBuffer
---
#### Summary
Clears out the buffer. Will cause performance issues if you do it too often.

StringDB.ThreadSafeDatabase.First
---
#### Summary
Gets the very first element in the database

StringDB.ThreadSafeDatabase.Flush
---
#### Summary
Flushes the prepending data to write to the stream

StringDB.ThreadSafeDatabase.FromDatabase(StringDB.IDatabase db)
---
#### Summary
Create a database that is thread safe by locking onto an object before any action

Param Name | Summary
---------- | -------
db | The database to make thread-safe

StringDB.ThreadSafeDatabase.Get&lt;TParam1&gt;(&lt;TParam1&gt;)
---
#### Summary
Gets the ReaderPair responsible for a given index

StringDB.ThreadSafeDatabase.Get&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;)
---
#### Summary
Gets the ReaderPair responsible for a given index

StringDB.ThreadSafeDatabase.GetAll&lt;TParam1&gt;(&lt;TParam1&gt;)
---
#### Summary
Gets the multiple ReaderPairs responsible for a given index

StringDB.ThreadSafeDatabase.GetAll&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;)
---
#### Summary
Gets the multiple ReaderPairs responsible for a given index

StringDB.ThreadSafeDatabase.GetEnumerator
---
StringDB.ThreadSafeDatabase.Insert&lt;TParam1, TParam2&gt;(&lt;TParam1&gt;, &lt;TParam2&gt;)
---
#### Summary
Insert an item into the database

StringDB.ThreadSafeDatabase.Insert&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, &lt;TParam1&gt;, &lt;TParam2&gt;)
---
#### Summary
Insert an item into the database

StringDB.ThreadSafeDatabase.Insert&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;})
---
#### Summary
Insert an item into the database

StringDB.ThreadSafeDatabase.Insert&lt;TParam1, TParam2&gt;(System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;})
---
#### Summary
Insert an item into the database

StringDB.ThreadSafeDatabase.InsertRange&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;}})
---
#### Summary
Insert multiple items into the database.

StringDB.ThreadSafeDatabase.InsertRange&lt;TParam1, TParam2&gt;(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;}})
---
#### Summary
Insert multiple items into the database.

StringDB.ThreadSafeDatabase.OverwriteValue&lt;TParam1&gt;(StringDB.Reader.IReaderPair, &lt;TParam1&gt;)
---
#### Summary
Overwrite a value. Note: You should call the database cleaning functions if you do this too frequently.

StringDB.ThreadSafeDatabase.OverwriteValue&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.Reader.IReaderPair, &lt;TParam1&gt;)
---
#### Summary
Overwrite a value. Note: You should call the database cleaning functions if you do this too frequently.

StringDB.ThreadSafeDatabase.System#Collections#IEnumerable#GetEnumerator
---
StringDB.ThreadSafeDatabase.TryGet&lt;TParam1&gt;(&lt;TParam1&gt;, StringDB.Reader.IReaderPair@)
---
#### Summary
Attempts to get the ReaderPair

StringDB.ThreadSafeDatabase.TryGet&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, &lt;TParam1&gt;, StringDB.Reader.IReaderPair@)
---
#### Summary
Attempts to get the ReaderPair

StringDB.TypeHandler&lt;TClass1&gt;.Compare(&lt;TClass1&gt; item1, &lt;TClass1&gt; item2)
---
#### Summary
Compare if two items are the same.

Param Name | Summary
---------- | -------
item1 | The first item to compare
item2 | The second item to compare

StringDB.TypeHandler&lt;TClass1&gt;.GetLength(&lt;TClass1&gt; item)
---
#### Summary
Gets the length of an item, or how long it would be when attempting to store it.

Param Name | Summary
---------- | -------
item | The item to calculate the length for.

StringDB.TypeHandler&lt;TClass1&gt;.Read(System.IO.BinaryReader br, System.Int64 len)
---
#### Summary
Read back the item from a stream, given the length of it. If you're not using the length of it, there's a good chance you're doing something wrong.

Param Name | Summary
---------- | -------
br | The BinaryReader.
len | The length of the data.

StringDB.TypeHandler&lt;TClass1&gt;.Write(System.IO.BinaryWriter bw, &lt;TClass1&gt; item)
---
#### Summary
Write an object to a BinaryWriter. The BinaryWriter should only be used to write the necessary data, and no seeking should be done. All you need to do is write the data, writing the length of the data will be taken care of for you assuming that the GetLength method is implemented properly.

Param Name | Summary
---------- | -------
bw | The BinaryWriter to use.
item | The item to write.

StringDB.TypeManager.GetHandlerFor(System.Byte id)
---
#### Summary
Returns the TypeHandler given a unique byte identifier.

Param Name | Summary
---------- | -------
id | The TypeHandler for the given byte Id

StringDB.TypeManager.GetHandlerFor&lt;TParam1&gt;
---
#### Summary
Get the type handler for a type

Param Name | Summary
---------- | -------
T | The type of type handler

StringDB.TypeManager.OverridingRegisterType&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;} t)
---
#### Summary
Overrides an existing type register, or adds it if it doesn't exist.

Param Name | Summary
---------- | -------
t | The type to override

StringDB.TypeManager.RegisterType&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;} t)
---
#### Summary
Register a type

Param Name | Summary
---------- | -------
t | The type to register

StringDB.Writer.IWriter.Flush
---
#### Summary
Flushes the prepending data to write to the stream

StringDB.Writer.IWriter.Insert&lt;TParam1, TParam2&gt;(&lt;TParam1&gt;, &lt;TParam2&gt;)
---
#### Summary
Insert an item into the database

StringDB.Writer.IWriter.Insert&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, &lt;TParam1&gt;, &lt;TParam2&gt;)
---
#### Summary
Insert an item into the database

StringDB.Writer.IWriter.Insert&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;})
---
#### Summary
Insert an item into the database

StringDB.Writer.IWriter.Insert&lt;TParam1, TParam2&gt;(System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;})
---
#### Summary
Insert an item into the database

StringDB.Writer.IWriter.InsertRange&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;}})
---
#### Summary
Insert multiple items into the database.

StringDB.Writer.IWriter.InsertRange&lt;TParam1, TParam2&gt;(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;}})
---
#### Summary
Insert multiple items into the database.

StringDB.Writer.IWriter.OverwriteValue&lt;TParam1&gt;(StringDB.Reader.IReaderPair, &lt;TParam1&gt;)
---
#### Summary
Overwrite a value. Note: You should call the database cleaning functions if you do this too frequently.

StringDB.Writer.IWriter.OverwriteValue&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.Reader.IReaderPair, &lt;TParam1&gt;)
---
#### Summary
Overwrite a value. Note: You should call the database cleaning functions if you do this too frequently.

StringDB.Writer.Writer.#ctor(System.IO.Stream s)
---
#### Summary
Create a new Writer.

Param Name | Summary
---------- | -------
s | The stream

StringDB.Writer.Writer.Flush
---
#### Summary
Flushes the prepending data to write to the stream

StringDB.Writer.Writer.Insert&lt;TParam1, TParam2&gt;(&lt;TParam1&gt;, &lt;TParam2&gt;)
---
#### Summary
Insert an item into the database

StringDB.Writer.Writer.Insert&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, &lt;TParam1&gt;, &lt;TParam2&gt;)
---
#### Summary
Insert an item into the database

StringDB.Writer.Writer.Insert&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;})
---
#### Summary
Insert an item into the database

StringDB.Writer.Writer.Insert&lt;TParam1, TParam2&gt;(System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;})
---
#### Summary
Insert an item into the database

StringDB.Writer.Writer.InsertRange&lt;TParam1, TParam2&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.TypeHandler{&lt;TParam2&gt;}, System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;}})
---
#### Summary
Insert multiple items into the database.

StringDB.Writer.Writer.InsertRange&lt;TParam1, TParam2&gt;(System.Collections.Generic.IEnumerable{System.Collections.Generic.KeyValuePair{&lt;TParam1&gt;, &lt;TParam2&gt;}})
---
#### Summary
Insert multiple items into the database.

StringDB.Writer.Writer.OverwriteValue&lt;TParam1&gt;(StringDB.Reader.IReaderPair, &lt;TParam1&gt;)
---
#### Summary
Overwrite a value. Note: You should call the database cleaning functions if you do this too frequently.

StringDB.Writer.Writer.OverwriteValue&lt;TParam1&gt;(StringDB.TypeHandler{&lt;TParam1&gt;}, StringDB.Reader.IReaderPair, &lt;TParam1&gt;)
---
#### Summary
Overwrite a value. Note: You should call the database cleaning functions if you do this too frequently.

## Properties

StringDB.ITypeHandler.Id
---
#### Summary
Unique byte identifier. Set it above 0x2F to avoid colliding with the predefined types.

StringDB.ITypeHandler.Type
---
#### Summary
Returns whatever type it is, typeof(T)

StringDB.Reader.IPart.InitialByte
---
#### Summary
The initial byte detected

StringDB.Reader.IPart.NextPart
---
#### Summary
Where the next position will be

StringDB.Reader.IPart.Position
---
#### Summary
position of this part

StringDB.Reader.IReaderPair.ByteArrayIndex
---
#### Summary
Get the index as a byte array instead.

StringDB.Reader.IReaderPair.DataPosition
---
#### Summary
The position of where the data is stored for this ReaderPair

StringDB.Reader.IReaderPair.Index
---
#### Summary
Whatever the index is.

StringDB.Reader.IReaderPair.Position
---
#### Summary
The position in the file that this ReaderPair is located at

StringDB.Reader.IReaderPair.ValueLength
---
#### Summary
Get how long the value is without reading it into memory.

StringDB.Reader.ReaderEnumerator.Current
---
#### Summary
What the current element is on.

StringDB.Reader.ReaderPair.ByteArrayIndex
---
#### Summary
Get the index as a byte array instead.

StringDB.Reader.ReaderPair.DataPosition
---
#### Summary
The position of where the data is stored for this ReaderPair

StringDB.Reader.ReaderPair.Index
---
#### Summary
Whatever the index is.

StringDB.Reader.ReaderPair.Position
---
#### Summary
The position in the file that this ReaderPair is located at

StringDB.Reader.ReaderPair.ValueLength
---
#### Summary
Get how long the value is without reading it into memory.

StringDB.Reader.ThreadSafeReaderPair.ByteArrayIndex
---
#### Summary
Get the index as a byte array instead.

StringDB.Reader.ThreadSafeReaderPair.DataPosition
---
#### Summary
The position of where the data is stored for this ReaderPair

StringDB.Reader.ThreadSafeReaderPair.Index
---
#### Summary
Whatever the index is.

StringDB.Reader.ThreadSafeReaderPair.Position
---
#### Summary
The position in the file that this ReaderPair is located at

StringDB.Reader.ThreadSafeReaderPair.ValueLength
---
#### Summary
Get how long the value is without reading it into memory.

StringDB.TypeHandler&lt;TClass1&gt;.Id
---
#### Summary
Unique byte identifier. Set it above 0x2F to avoid colliding with the predefined types.

StringDB.TypeHandler&lt;TClass1&gt;.Type
---
#### Summary
Returns whatever type it is, typeof(T)

## Fields

StringDB.Consts.DeletedValue
---
#### Summary
Used to tell that this value is deleted.

StringDB.Consts.IndexSeperator
---
#### Summary
Used for seperating indexes from data. This is why you can't have indexes with lengths more then 253.

StringDB.Consts.IsByteValue
---
#### Summary
Used to tell if the next value is a byte

StringDB.Consts.IsLongValue
---
#### Summary
Used to tell if the next value is a ulong

StringDB.Consts.IsUIntValue
---
#### Summary
Used to tell if the next value is a uint

StringDB.Consts.IsUShortValue
---
#### Summary
Used to tell if the next value is a ushort

StringDB.Consts.MaxLength
---
#### Summary
The maximum value for an index. After this point, bytes are 

StringDB.Consts.NoIndex
---
#### Summary
An index with zero length tells it that there's nothing after this.

