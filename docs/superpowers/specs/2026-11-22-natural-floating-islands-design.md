# Natural Island Tops Design

## Intent

Pertahankan bentuk bawah Terrain normal pada lima pulau, tetapi buat permukaan atas lebih alami dengan gundukan lembut dan vegetasi.

## Islands

- `Math_Entry_Island`
- `Math_Finish_Island`
- `Math_High_Island`
- `Math_Upper_Arc_Island`
- `Final_Center_Island`

## Shape

- Terrain tidak memakai holes dan tetap memiliki dasar/sisi bawaan yang utuh.
- Area gameplay penting tetap datar: gate, exit Math, dan tengah finish besar.
- Area luar plateau mendapat gundukan dan cekungan lembut.
- Tidak ada mesh, collider, atau material underside tambahan.
- Rumput, batu, dan pohon tetap berada di area luar plateau.

## Constraints

- Jangan menjalankan full Math builder.
- Pertahankan 5 `Parkour_X_*`, 30 jawaban, satu Water, dan `SciFiGunLight`.
- Tidak menambah akses, bridge, checkpoint, platform, atau logika finish.
- Auto-builder tetap mati.

## Approach

- Dipilih: Terrain saja. Seluruh hole mask dan `Natural_Underside` dihapus; noise permukaan tetap dilindungi dari plateau gameplay.
- Ditolak: mesh underside karena bentuk dan materialnya tidak sesuai visual yang diminta.
- Ditolak: full custom mesh karena tidak dibutuhkan untuk gundukan permukaan.
