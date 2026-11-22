# Outer Arc and Finish Islands Design

## Intent

Tambah dua pulau melayang setelah `Math_High_Island`: satu pulau transisi yang lebih tinggi pada jalur melengkung, lalu satu pulau finish besar di tengah arena.

## Layout

- `Math_Upper_Arc_Island`: pusat sekitar `(-90, 59, 90)`, ukuran sekitar `28 x 30`.
- `Final_Center_Island`: pusat sekitar `(0, 68, 80)`, ukuran sekitar `44 x 44`.
- Susunan pulau luar membentuk busur, bukan garis atau sudut kotak kaku.
- Kedua pulau melayang di atas air.

## Visual

- Pulau transisi: 3 pohon, rumput, dan batu di tepian.
- Pulau finish: 5–6 pohon, rumput, dan batu di tepian.
- Area tengah pulau finish luas dan kosong untuk gameplay berikutnya.

## Scope

- Belum ada akses, bridge, platform, checkpoint, atau logika finish.
- Pertahankan Math Gate, tiga pulau saat ini, senjata, Water, dan edit block manual.
- Generator tetap manual; perubahan script tidak boleh membangun ulang scene otomatis.

## Approach

- Dipilih: loop kotak lembut dengan busur luar dan finish besar di tengah.
- Ditolak: loop luar penuh karena jarak terlalu jauh.
- Ditolak: garis diagonal karena susunan arena kurang mudah dibaca.
