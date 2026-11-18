Add-Type -AssemblyName System.Drawing
$out = Join-Path $PSScriptRoot '..\Assets\AR\Marcadores'
New-Item -ItemType Directory -Force $out | Out-Null

$cards = @(
  @{ n = 'Sol';     c = 'Orange';    seed = 11 },
  @{ n = 'Tierra';  c = 'RoyalBlue'; seed = 22 },
  @{ n = 'Marte';   c = 'Firebrick'; seed = 33 },
  @{ n = 'Saturno'; c = 'Goldenrod'; seed = 44 }
)
$W = 800; $H = 1100

foreach ($card in $cards) {
  $rnd = New-Object System.Random $card.seed
  $bmp = New-Object System.Drawing.Bitmap $W, $H
  $g = [System.Drawing.Graphics]::FromImage($bmp)
  $g.SmoothingMode = 'AntiAlias'
  $g.Clear([System.Drawing.Color]::White)
  $main = [System.Drawing.Color]::FromName($card.c)

  for ($i = 0; $i -lt 140; $i++) {
    $col = if ($rnd.Next(2)) { [System.Drawing.Color]::Black } else { $main }
    $b = New-Object System.Drawing.SolidBrush $col
    $x = $rnd.Next(-40, $W); $y = $rnd.Next(-40, $H); $s = $rnd.Next(12, 90)
    switch ($rnd.Next(3)) {
      0 { $g.FillEllipse($b, $x, $y, $s, $s) }
      1 { $g.FillRectangle($b, $x, $y, $s, [int]($s / 2)) }
      2 { $g.FillPolygon($b, [System.Drawing.Point[]]@(
            (New-Object System.Drawing.Point $x, $y),
            (New-Object System.Drawing.Point ($x + $s), ($y + $rnd.Next(-30, 30))),
            (New-Object System.Drawing.Point ($x + $rnd.Next(-30, 30)), ($y + $s)))) }
    }
    $b.Dispose()
  }

  $pb = New-Object System.Drawing.SolidBrush $main
  $g.FillEllipse([System.Drawing.Brushes]::Black, 190, 290, 420, 420)
  $g.FillEllipse($pb, 200, 300, 400, 400)
  for ($i = 0; $i -lt 12; $i++) {
    $s = $rnd.Next(20, 70)
    $g.FillEllipse([System.Drawing.Brushes]::Black, $rnd.Next(250, 520), $rnd.Next(350, 600), $s, $s)
  }
  $g.FillRectangle([System.Drawing.Brushes]::White, 0, 880, $W, 160)
  $g.DrawRectangle((New-Object System.Drawing.Pen ([System.Drawing.Color]::Black), 6), 3, 883, $W - 6, 154)
  $font = New-Object System.Drawing.Font 'Arial Black', 72, ([System.Drawing.FontStyle]::Bold)
  $fmt = New-Object System.Drawing.StringFormat; $fmt.Alignment = 'Center'
  $g.DrawString($card.n.ToUpper(), $font, [System.Drawing.Brushes]::Black, (New-Object System.Drawing.RectangleF 0, 905, $W, 120), $fmt)
  $g.DrawRectangle((New-Object System.Drawing.Pen ([System.Drawing.Color]::Black), 16), 8, 8, $W - 16, $H - 16)

  $path = Join-Path $out ($card.n + '.png')
  $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
  $g.Dispose(); $bmp.Dispose(); $pb.Dispose()
  "OK $path"
}
