<?xml version="1.0" encoding="utf-8"?>
<gml:FeatureCollection xmlns:gml="http://www.opengis.net/gml/3.2"
  xmlns:app="http://skjema.geonorge.no/SOSI/produktspesifikasjon/Reguleringsplanforslag/5.0"
  xmlns:sc="http://www.interactive-instruments.de/ShapeChange/AppInfo"
  xmlns="http://www.w3.org/2001/XMLSchema"
  xmlns:xlink="http://www.w3.org/1999/xlink"
  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:schemaLocation="http://skjema.geonorge.no/SOSI/produktspesifikasjon/Reguleringsplanforslag/5.0 http://skjema.geonorge.no/SOSITEST/produktspesifikasjon/Reguleringsplanforslag/5.0/reguleringsplanforslag-5.0.xsd" gml:id="_60b01995-7434-4d77-bc58-d64344f30749">
  <gml:boundedBy>
    <gml:Envelope srsDimension="2" srsName="http://www.opengis.net/def/crs/EPSG/0/25832">
      <gml:lowerCorner>-13546.2606568071 196462.35</gml:lowerCorner>
      <gml:upperCorner>-12890.0410483495 197000.53032463</gml:upperCorner>
    </gml:Envelope>
  </gml:boundedBy>
  <gml:featureMember>
    <app:Arealplan gml:id="_e62e0d8a-7a86-4fa6-b35b-b7a319081fd3"
      xmlns:app="http://skjema.geonorge.no/SOSI/produktspesifikasjon/Reguleringsplanforslag/5.0">
      <app:identifikasjon>
        <app:Identifikasjon>
          <app:lokalId>41c29195-9926-45b0-99e2-eaba686ee290</app:lokalId>
          <app:navnerom>http://data.geonorge.no/0301/Reguleringsplaner/so</app:navnerom>
          <app:versjonId>0d0f7110-148d-4549-ba25-c920e7c241e7</app:versjonId>
        </app:Identifikasjon>
      </app:identifikasjon>
      <app:oppdateringsdato>2001-12-17T09:30:47Z</app:oppdateringsdato>
      <app:prosesshistorie>prosess</app:prosesshistorie>
      <app:informasjon>informasjon</app:informasjon>
      <app:link>link</app:link>
      <app:nasjonalArealplanId>
        <app:NasjonalArealplanId>
          <app:administrativEnhet>
            <app:AdministrativEnhetskode>
              <app:kommunenummer>String</app:kommunenummer>
            </app:AdministrativEnhetskode>
          </app:administrativEnhet>
          <app:planidentifikasjon>String</app:planidentifikasjon>
        </app:NasjonalArealplanId>
      </app:nasjonalArealplanId>
      <app:plantype>34</app:plantype>
      <app:plannavn>String</app:plannavn>
      <app:planstatus>2</app:planstatus>
      <app:lovreferanse>6</app:lovreferanse>
      <app:rpOmråde xlink:type="simple" xlink:href="#_d23f6d0c-ac1d-4857-b2fc-a8501e014dfb" />
    </app:Arealplan>
  </gml:featureMember>
  <gml:featureMember>
    <app:RpOmråde gml:id="_d23f6d0c-ac1d-4857-b2fc-a8501e014dfb">
      <app:identifikasjon>
        <app:Identifikasjon>
          <app:lokalId>9f7ecfb2-02a2-4827-b11a-eb607a36ef4d</app:lokalId>
          <app:navnerom>http://data.geonorge.no/4203/Reguleringsplaner/so</app:navnerom>
          <app:versjonId>"2020-12-22 12:41:36.000000"</app:versjonId>
        </app:Identifikasjon>
      </app:identifikasjon>
      <app:oppdateringsdato>2021-03-09T11:22:38.9095372+01:00</app:oppdateringsdato>
      <app:opphav>Lorem ipsum</app:opphav>
      <app:område>
        <gml:MultiSurface gml:id="_d23f6d0c-ac1d-4857-b2fc-a8501e014dfb-0">
          <gml:surfaceMember>
            <gml:Polygon gml:id="_d23f6d0c-ac1d-4857-b2fc-a8501e014dfb-1">
              <gml:exterior>
                <gml:LinearRing>
                  <gml:posList>4.77338810569873 -0.37657919317042 4.79561302269819 -0.059874125928085 4.801689416227 0.026714481857456 4.9785726070287 0.415857501621197 5.38894160968865 0.76254855559253 6.08939904526338 0.960657729290434 6.16856046619247 0.960499089168331 9.61998753366532 0.953582401658366 9.58461089550498 3.86154205843832 7.2356021216584 3.81909009264591 7.24267744929047 3.16815995049565 8.55868838885512 3.2247625715522 8.57991437175132 1.95827892541202 6.74032918741364 1.94412827014789 6.74032918741364 2.46005790805409 6.74032918741364 3.81909009264591 4.4479230346236 3.7766381268535 4.46207368988774 1.01018502271491 3.93849944511471 1.01466001625998 3.63426035693578 1.01726035034698 3.62718502930371 2.81439356889225 2.31824941737113 2.12808678858165 2.304098762107 -0.90722876557552 4.79461408859494 -0.914304093207588 4.77338810569873 -0.37657919317042</gml:posList>
                </gml:LinearRing>
              </gml:exterior>
            </gml:Polygon>
          </gml:surfaceMember>
        </gml:MultiSurface>
      </app:område>
      <app:vertikalnivå>5</app:vertikalnivå>
      <app:vertikallag>
        <app:Vertikallag>
          <app:lag>Lorem ipsum</app:lag>
          <app:referansehøyde>23.32</app:referansehøyde>
        </app:Vertikallag>
      </app:vertikallag>
      <app:arealplan xlink:type="simple" xlink:href="#_e62e0d8a-7a86-4fa6-b35b-b7a319081fd3" />
      <app:formål xlink:type="simple" xlink:href="#_1deb7804-8671-469f-a0af-0ce6d5fa0b5c" />
      <app:formål xlink:type="simple" xlink:href="#_23f6ea2a-0943-4d5f-94d2-0b68ad85f929" />
      <app:formål xlink:type="simple" xlink:href="#_6d6d42da-21f9-4263-8413-27171ff10303" />
    </app:RpOmråde>
  </gml:featureMember>
  <gml:featureMember>
    <app:RpArealformålOmråde gml:id="_1deb7804-8671-469f-a0af-0ce6d5fa0b5c"
      xmlns:app="http://skjema.geonorge.no/SOSI/produktspesifikasjon/Reguleringsplanforslag/5.0">
      <app:identifikasjon>
        <app:Identifikasjon>
          <app:lokalId>12063deb-d393-4e7d-87f8-1709adfec500</app:lokalId>
          <app:navnerom>http://data.geonorge.no/0301/Reguleringsplaner/so</app:navnerom>
          <app:versjonId>56f46a19-c47a-4dda-97e8-4dc8bc9cfb7b</app:versjonId>
        </app:Identifikasjon>
      </app:identifikasjon>
      <app:førsteDigitaliseringsdato>2020-12-22T00:00:00.000</app:førsteDigitaliseringsdato>
      <app:oppdateringsdato>2020-12-22T00:00:00.000</app:oppdateringsdato>
      <app:kvalitet>
        <app:Posisjonskvalitet>
          <app:målemetode>frihåndstegningPåSkjerm</app:målemetode>
          <app:nøyaktighet>0</app:nøyaktighet>
        </app:Posisjonskvalitet>
      </app:kvalitet>
      <app:område>
        <gml:MultiSurface gml:id="_1deb7804-8671-469f-a0af-0ce6d5fa0b5c-0">
          <gml:surfaceMember>
            <gml:Polygon gml:id="_1deb7804-8671-469f-a0af-0ce6d5fa0b5c-1">
              <gml:exterior>
                <gml:LinearRing>
                  <gml:posList>3.93849944511471 1.01466001625998 3.63426035693578 1.01726035034698 3.62718502930371 2.81439356889225 2.31824941737113 2.12808678858165 2.304098762107 -0.90722876557552 4.79461408859494 -0.914304093207588 4.77338810569873 -0.37657919317042 4.79561302269819 -0.059874125928085 3.93849944511471 -0.065264777359427 3.93849944511471 1.01466001625998</gml:posList>
                </gml:LinearRing>
              </gml:exterior>
            </gml:Polygon>
          </gml:surfaceMember>
        </gml:MultiSurface>
      </app:område>
      <app:arealformål>1169</app:arealformål>
      <app:feltnavn>o_BTAN1</app:feltnavn>
      <app:beskrivelse>Beskrivelse</app:beskrivelse>
      <app:eierform>1</app:eierform>
      <app:utnytting>
        <app:Utnytting>
          <app:utnyttingstype>16</app:utnyttingstype>
          <app:utnyttingstall>43</app:utnyttingstall>
          <app:utnyttingstall_minimum>20</app:utnyttingstall_minimum>
        </app:Utnytting>
      </app:utnytting>
      <app:uteoppholdsareal>0</app:uteoppholdsareal>
      <app:planområde xlink:type="simple" xlink:href="#_d23f6d0c-ac1d-4857-b2fc-a8501e014dfb" />
    </app:RpArealformålOmråde>
  </gml:featureMember>
  <gml:featureMember>
    <app:RpArealformålOmråde gml:id="_23f6ea2a-0943-4d5f-94d2-0b68ad85f929"
      xmlns:app="http://skjema.geonorge.no/SOSI/produktspesifikasjon/Reguleringsplanforslag/5.0">
      <app:identifikasjon>
        <app:Identifikasjon>
          <app:lokalId>614a1fde-4afa-4b56-94fa-b9fa8af72f43</app:lokalId>
          <app:navnerom>http://data.geonorge.no/0301/Reguleringsplaner/so</app:navnerom>
          <app:versjonId>56f46a19-c47a-4dda-97e8-4dc8bc9cfb7b</app:versjonId>
        </app:Identifikasjon>
      </app:identifikasjon>
      <app:førsteDigitaliseringsdato>2020-12-22T00:00:00.000</app:førsteDigitaliseringsdato>
      <app:oppdateringsdato>2020-12-22T00:00:00.000</app:oppdateringsdato>
      <app:kvalitet>
        <app:Posisjonskvalitet>
          <app:målemetode>frihåndstegningPåSkjerm</app:målemetode>
          <app:nøyaktighet>0</app:nøyaktighet>
        </app:Posisjonskvalitet>
      </app:kvalitet>
      <app:område>
        <gml:MultiSurface gml:id="_23f6ea2a-0943-4d5f-94d2-0b68ad85f929-0">
          <gml:surfaceMember>
            <gml:Polygon gml:id="_23f6ea2a-0943-4d5f-94d2-0b68ad85f929-1">
              <gml:exterior>
                <gml:LinearRing>
                  <gml:posList>6.74032918741364 2.46005790805409 6.74032918741364 3.81909009264591 4.4479230346236 3.7766381268535 4.46207368988774 1.01018502271491 3.93849944511471 1.01466001625998 3.93849944511471 -0.065264777359427 4.79561302269819 -0.059874125928085 4.801689416227 0.026714481857456 4.9785726070287 0.415857501621197 5.38894160968865 0.76254855559253 6.08939904526338 0.960657729290434 6.16856046619247 0.960499089168331 6.1743029768482 2.45355185965678 6.74032918741364 2.46005790805409</gml:posList>
                </gml:LinearRing>
              </gml:exterior>
            </gml:Polygon>
          </gml:surfaceMember>
        </gml:MultiSurface>
      </app:område>
      <app:arealformål>1169</app:arealformål>
      <app:feltnavn>o_BTAN2</app:feltnavn>
      <app:beskrivelse>Beskrivelse</app:beskrivelse>
      <app:eierform>1</app:eierform>
      <app:utnytting>
        <app:Utnytting>
          <app:utnyttingstype>16</app:utnyttingstype>
          <app:utnyttingstall>43</app:utnyttingstall>
          <app:utnyttingstall_minimum>20</app:utnyttingstall_minimum>
        </app:Utnytting>
      </app:utnytting>
      <app:uteoppholdsareal>0</app:uteoppholdsareal>
      <app:planområde xlink:type="simple" xlink:href="#_d23f6d0c-ac1d-4857-b2fc-a8501e014dfb" />
    </app:RpArealformålOmråde>
  </gml:featureMember>
  <gml:featureMember>
    <app:RpArealformålOmråde gml:id="_6d6d42da-21f9-4263-8413-27171ff10303"
      xmlns:app="http://skjema.geonorge.no/SOSI/produktspesifikasjon/Reguleringsplanforslag/5.0">
      <app:identifikasjon>
        <app:Identifikasjon>
          <app:lokalId>f1d43920-1895-41ad-b33f-8c3b571cb873</app:lokalId>
          <app:navnerom>http://data.geonorge.no/0301/Reguleringsplaner/so</app:navnerom>
          <app:versjonId>56f46a19-c47a-4dda-97e8-4dc8bc9cfb7b</app:versjonId>
        </app:Identifikasjon>
      </app:identifikasjon>
      <app:førsteDigitaliseringsdato>2020-12-22T00:00:00.000</app:førsteDigitaliseringsdato>
      <app:oppdateringsdato>2020-12-22T00:00:00.000</app:oppdateringsdato>
      <app:kvalitet>
        <app:Posisjonskvalitet>
          <app:målemetode>frihåndstegningPåSkjerm</app:målemetode>
          <app:nøyaktighet>0</app:nøyaktighet>
        </app:Posisjonskvalitet>
      </app:kvalitet>
      <app:område>
        <gml:MultiSurface gml:id="_6d6d42da-21f9-4263-8413-27171ff10303-0">
          <gml:surfaceMember>
            <gml:Polygon gml:id="_6d6d42da-21f9-4263-8413-27171ff10303-1">
              <gml:exterior>
                <gml:LinearRing>
                  <gml:posList>6.16856046619247 0.960499089168331 9.61998753366532 0.953582401658366 9.58461089550498 3.86154205843832 7.2356021216584 3.81909009264591 7.24267744929047 3.16815995049565 8.55868838885512 3.2247625715522 8.57991437175132 1.95827892541202 6.74032918741364 1.94412827014789 6.74032918741364 2.46005790805409 6.1743029768482 2.45355185965678 6.16856046619247 0.960499089168331</gml:posList>
                </gml:LinearRing>
              </gml:exterior>
            </gml:Polygon>
          </gml:surfaceMember>
        </gml:MultiSurface>
      </app:område>
      <app:arealformål>1169</app:arealformål>
      <app:feltnavn>o_BTAN3</app:feltnavn>
      <app:beskrivelse>Beskrivelse</app:beskrivelse>
      <app:eierform>1</app:eierform>
      <app:utnytting>
        <app:Utnytting>
          <app:utnyttingstype>16</app:utnyttingstype>
          <app:utnyttingstall>104</app:utnyttingstall>
          <app:utnyttingstall_minimum>20</app:utnyttingstall_minimum>
        </app:Utnytting>
      </app:utnytting>
      <app:uteoppholdsareal>0</app:uteoppholdsareal>
      <app:planområde xlink:type="simple" xlink:href="#_d23f6d0c-ac1d-4857-b2fc-a8501e014dfb" />
    </app:RpArealformålOmråde>
  </gml:featureMember>
</gml:FeatureCollection>