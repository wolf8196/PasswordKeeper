using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PasswordKeeper.Utils;
using StreamIndexingUtils;
using StreamIndexingUtils.Models;

namespace PasswordKeeper.Security
{
    public sealed partial class CryptoStorage : ICryptoStorage
    {
        private readonly Func<Stream, Stream, Task> encryptionTransformation;
        private readonly Func<Stream, Stream, Task> decryptionTransformation;

        private IKeyDerivationFunction kdf;
        private IEncryptor encryptor;

        private byte[] accessKey;

        public CryptoStorage(IKeyDerivationFunction kdf, IEncryptor encryptor, Stream dataSource)
            : this(kdf, encryptor)
        {
            DataSource = dataSource;
        }

        public CryptoStorage(IKeyDerivationFunction kdf, IEncryptor encryptor)
            : this()
        {
            kdf.ThrowIfNull(nameof(kdf));
            encryptor.ThrowIfNull(nameof(encryptor));

            this.kdf = kdf;
            this.encryptor = encryptor;
        }

        internal CryptoStorage()
        {
            encryptionTransformation = (src, dest) => encryptor.EncryptAsync(src, dest, accessKey);
            decryptionTransformation = (src, dest) => encryptor.DecryptAsync(src, dest, accessKey);
        }

        public Stream DataSource { get; set; }

        public bool IsOpened { get => accessKey != null; }

        public IEnumerable<string> Content
        {
            get
            {
                return ContentIndex?.Keys.ToList();
            }
        }

        private ContentIndex ContentIndex { get; set; }

        public async Task SetupAsync(byte[] key)
        {
            key.ThrowIfNull(nameof(key));
            DataSource.ThrowIfNull(nameof(DataSource));

            ContentIndex = new ContentIndex(DataSource.Position);
            accessKey = kdf.Generate(key, encryptor.KeySize);

            await SaveAsync().ConfigureAwait(false);
        }

        public async Task<bool> OpenAsync(byte[] key)
        {
            key.ThrowIfNull(nameof(key));
            DataSource.ThrowIfNull(nameof(DataSource));

            try
            {
                accessKey = kdf.Generate(key, encryptor.KeySize);
                ContentIndex = await new IndexSerializer().DeserializeAsync(DataSource, decryptionTransformation).ConfigureAwait(false);
                return true;
            }
            catch (CipherTextAuthenticationException)
            {
                Dispose();
            }
            catch (FormatException)
            {
                Dispose();
            }

            return false;
        }

        public async Task GetAsync(string id, Stream destination)
        {
            id.ThrowIfNullOrEmpty(nameof(id));
            destination.ThrowIfNull(nameof(destination));
            DataSource.ThrowIfNull(nameof(DataSource));
            Authorize();

            using (var reader = new IndexedReadOnlyStream(DataSource, ContentIndex, id, true))
            {
                await encryptor.DecryptAsync(reader, destination, accessKey).ConfigureAwait(false);
            }
        }

        public async Task AddAsync(string id, Stream source)
        {
            id.ThrowIfNullOrEmpty(nameof(id));
            source.ThrowIfNull(nameof(source));
            DataSource.ThrowIfNull(nameof(DataSource));
            Authorize();

            using (var writer = new IndexedWriteStream(DataSource, ContentIndex, id, true))
            {
                await encryptor.EncryptAsync(source, writer, accessKey).ConfigureAwait(false);
            }
        }

        public async Task RemoveAsync(string id)
        {
            id.ThrowIfNullOrEmpty(nameof(id));
            DataSource.ThrowIfNull(nameof(DataSource));
            Authorize();

            using (var idxReaderWriter = new IndexedStreamReaderWriter(DataSource, ContentIndex, true))
            {
                await idxReaderWriter.RemoveAsync(id).ConfigureAwait(false);
            }
        }

        public async Task SaveAsync()
        {
            DataSource.ThrowIfNull(nameof(DataSource));
            Authorize();

            await new IndexSerializer().SerializeAsync(ContentIndex, DataSource, encryptionTransformation).ConfigureAwait(false);
        }

        public ISerializer GetSerializer()
        {
            return new Serializer(this);
        }

        public IMemento GetMemento()
        {
            return new Memento(this);
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (accessKey != null)
            {
                Array.Clear(accessKey, 0, accessKey.Length);
                accessKey = null;
            }

            ContentIndex = null;
        }

        private void Authorize()
        {
            if (!IsOpened)
            {
                throw new InvalidOperationException("Storage is closed. Call OpenAsync first");
            }
        }
    }
}