using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

internal class Day3 : Day
{

    public string Star1(string input, bool example = false)
    {
        var matches = Regex.Matches(input, @"mul\((\d+),(\d+)\)");
        return matches.Select(m => int.Parse(m.Groups[1].Value) * int.Parse(m.Groups[2].Value)).Sum().ToString();
    }
    public string Star2(string input, bool example = false)
    {
        if(example) input = "don't()select();@??]?mul(693,878)from()when()@/mul(202,438)mul(266,957)from()])[/how()select()don't()+>mul(741,967)select()what()+[[why()&]mul(42,517)>]why()<mul(577,659)&['<where()>,select(297,821):mul(324,191);%{->:!#mul(567,73)don't()?why():^{$*]why()mul(504,229)'mul(40,787)/select()?who();[who()where();$mul(537,42)[#@&*~<@mul(418,58);{}how()*-don't()[<'@from() mul(38,329)-*mul(381,685)what()</:)^(',when()mul(850,565)@,,%select(95,745)#mul(99,343),]^who()[mul(46,377)mul(788,562)?<mul(101,829)]$#-]select()mul(536,292)who(419,754)mul(933:[>?+,,*%)mul(740,7)#)<how()mul(44,988)mul(693,770)why()-;[mul(76,905)where()];mul(457,100)what()mul(63,750)where()!&who()$>#)?^mul(211,355){~#who()<]who()@>~mul(135,667)-^)]'mul(165,161)<;,mul(350,311);%?~}$how(768,325)*mul(773,359)~!(select()?{why()mul(269,149)when()}*who()}mul(536,90)don't()>$ &!where()*who()&mul(405,425)!+mul(304,694)'<#$<<,#select()<mulselect()<?mul(448,840)!]-;mul(79,274)>):%:* -why()#mul[who()when(918,883){ ^''mul(907,968)what()when()[who()>mul(81'how()+what()select()<mul(66,242)mul(32,790)why()why()mul(707,867);{+[@select()@,mul(183,895)~mul(570)],:select(),%{%from()mul(568,881)[{@/}*mul(989,413)^($who()!}<&;mul(380,574)+why()?select()#mul(812,587)why()when()mul(772,647)>;&'']+how()mul(745,379)]^-mul(403,82)mul(840,830)when()+what()how()[!select()mul(827,564)?:]mul(602,333){what()[[mul(606,272)\r\n,who()>}who():#from()mul(960,322)where()'<from()why()what()%where()mul(282,700)mul(591,113)<where()%, )mul(17}{')~!-mul(462,619)]mul(422,776)mul(250,987){((mul(880,386)(<>)?,+when() mul(59,855)'mul(308,433)~<how()>[;<why(98,720)@*mul(406,466)-what()when()'do()";

        return Star1(Regex.Replace(input, @"don't\(\).*?(do\(\)|\Z)", "",RegexOptions.Singleline));
        //guess1  57372270 low 
        //guess2 126412926 high     added .*? lazy quantifier
        //guess3 125598543 high     added match end (do\(\)|$)
        //guess3 125598543 same?    fixed match end (do\(\)|\Z) to not match end of line
        //guess4 111762583 correct  RegexOptions.Singleline, the dot does not match linebreak otherwise (.|\n) could also work
    }


}