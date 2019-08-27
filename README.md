# PasswordKeeper
PasswordKeeper is a small and simple desktop password manager.
All the passwords and attachments are stored in single encrypted file.
The app is strictly offline. No network communication whatsoever.
That is why transfering of the database file is up to the user. Choose whatever method, you trust the most.

## Prerequisites
.NET Framework 4.7.2

## Security
Currently only one encryption algorithm and one key derivation function is supported, but more may be added in future.
### Encryption
1. **AES-256-CBC-PKCS7-HMACSHA-512** -- Advanced Encryption Standard algorithm with 256-bit key, CBC mode and PKCS7 padding. The authenticity and integrity is ensured using a HMAC-SHA-512 hash of the ciphertext (Encrypt-then-MAC scheme). (see [Aes256CbcHmacSha512.cs](https://github.com/wolf8196/PasswordKeeper/blob/master/PasswordKeeper.Security/Aes256CbcHmacSha512.cs))

### Key derivation
1. **PBKDF2-SHA512** -- Password-Based Key Derivation Function 2 with SHA-512 hashing algorithm and 64-bit random salt. Number of iterations is configurable during registration with default value of 100000. (see [Pbkdf2Sha512.cs](https://github.com/wolf8196/PasswordKeeper/blob/master/PasswordKeeper.Security/Pbkdf2Sha512.cs))

### Database file structure
The beginning of the file stores the plain public information required for encryption algorithm (e.g. algorithm id, initialization vector) and key derivation function (e.g. KDF id, salt, number of iterations).
After that goes the encrypted table of contents, attachments & passwords.
And in the end there is a plain 64-bit interger, that points to the begging of table of contents.

## Contribution
If you noticed a bug, issue, vulnerability or have good suggestions, please feel free to contact me.

## Acknowledgments
Many thanks to the creators of [MahApps.Metro](https://mahapps.com/). Nothing from the current user interface would have been possible without their toolkit.

## License
This project is licensed under the MIT License - see the [LICENSE](https://github.com/wolf8196/PasswordKeeper/blob/master/LICENSE) file for details.
