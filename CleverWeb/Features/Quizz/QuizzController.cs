using Microsoft.AspNetCore.Mvc;

namespace CleverWeb.Features.Quizz
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public int Answer { get; set; }
        public string Reference { get; set; } = string.Empty;
    }

    public class QuizzController : Controller
    {
        private static List<Question> Questions = new()
        {

        // Muito faceis
        new Question { Id = 1, Text = "1 - Quem construiu a arca?", Options = new(){"A) - Moisés", "B) - Noé", "C) - Abraão", "D) - Davi"}, Answer = 1, Reference = "Gênesis 6:14" },
        new Question { Id = 2, Text = "2 - Quem foi o primeiro homem criado por Deus?", Options = new(){"A) - Adão", "B) - Noé", "C) - Abraão", "D) - José"}, Answer = 0, Reference = "Gênesis 2:7" },
        new Question { Id = 3, Text = "3 - Quem abriu o Mar Vermelho?", Options = new(){"A) - Josué", "B) - Moisés", "C) - Elias", "D) - Davi"}, Answer = 1, Reference = "Êxodo 14:21" },
        new Question { Id = 4, Text = "4 - Quem matou o gigante Golias?", Options = new(){"A) - Saul", "B) - Davi", "C) - Samuel", "D) - Salomão"}, Answer = 1, Reference = "1 Samuel 17:49" },
        new Question { Id = 5, Text = "5 - Quem foi lançado na cova dos leões?", Options = new(){"A) - Daniel", "B) - Elias", "C) - Eliseu", "D) - Jeremias"}, Answer = 0, Reference = "Daniel 6:16" },
        new Question { Id = 6, Text = "6 - Quem engoliu Jonas?", Options = new(){"A) - Um leão", "B) - Um grande peixe", "C) - Um anjo", "D) - Um camelo"}, Answer = 1, Reference = "Jonas 1:17" },
        new Question { Id = 7, Text = "7 - Quem nasceu em Belém?", Options = new(){"A) - João Batista", "B) - Pedro", "C) - Jesus", "D) - Paulo"}, Answer = 2, Reference = "Mateus 2:1" },
        new Question { Id = 8, Text = "8 - Quantos dias choveu no dilúvio?", Options = new(){"A) - 7 dias", "B) - 12 dias", "C) - 40 dias", "D) - 100 dias"}, Answer = 2, Reference = "Gênesis 7:12" },
        new Question { Id = 9, Text = "9 - Quem traiu Jesus?", Options = new(){"A) - Pedro", "B) - João", "C) - Judas", "D) - Tiago"}, Answer = 2, Reference = "Mateus 26:14-16" },
        new Question { Id = 10, Text = "10 - Em quantos dias Jesus ressuscitou?", Options = new(){"A) - 1 dia", "B) - 2 dias", "C) - 3 dias", "D) - 7 dias"}, Answer = 2, Reference = "Mateus 28:6" },
      
        // Faceis
        new Question { Id = 11, Text = "11 - Quem recebeu as tábuas dos Dez Mandamentos?", Options = new(){"A) - Abraão", "B) - Moisés", "C) - Davi", "D) - Salomão"}, Answer = 1, Reference = "Êxodo 31:18" },
        new Question { Id = 12, Text = "12 - Quem foi engolido por um grande peixe?", Options = new(){"A) - Jonas", "B) - Elias", "C) - Eliseu", "D) - Jeremias"}, Answer = 0, Reference = "Jonas 1:17" },
        new Question { Id = 13, Text = "13 - Quem foi o pai de Isaque?", Options = new(){"A) - Jacó", "B) - Abraão", "C) - José", "D) - Noé"}, Answer = 1, Reference = "Gênesis 21:3" },
        new Question { Id = 14, Text = "14 - Quem interpretou os sonhos do faraó no Egito?", Options = new(){"A) - Moisés", "B) - Daniel", "C) - José", "D) - Arão"}, Answer = 2, Reference = "Gênesis 41:15-16" },
        new Question { Id = 15, Text = "15 - Qual discípulo andou sobre as águas com Jesus?", Options = new(){"A) - João", "B) - Pedro", "C) - Tiago", "D) - André"}, Answer = 1, Reference = "Mateus 14:29" },
        new Question { Id = 16, Text = "16 - Quem foi a mãe de Jesus?", Options = new(){"A) - Marta", "B) - Maria", "C) - Isabel", "D) - Ana"}, Answer = 1, Reference = "Lucas 1:30-31" },
        new Question { Id = 17, Text = "17 - Quem construiu o templo em Jerusalém?", Options = new(){"A) - Davi", "B) - Salomão", "C) - Saul", "D) - Ezequias"}, Answer = 1, Reference = "1 Reis 6:1" },
        new Question { Id = 18, Text = "18 - Quem foi lançado na fornalha ardente com seus amigos?", Options = new(){"A) - Daniel", "B) - Sadraque", "C) - Jeremias", "D) - Ezequiel"}, Answer = 1, Reference = "Daniel 3:20-23" },
        new Question { Id = 19, Text = "19 - Quem batizou Jesus?", Options = new(){"A) - Pedro", "B) - João Batista", "C) - Elias", "D) - Paulo"}, Answer = 1, Reference = "Mateus 3:13-17" },
        new Question { Id = 20, Text = "20 - Quantos discípulos Jesus escolheu?", Options = new(){"A) - 10", "B) - 11", "C) - 12", "D) - 70"}, Answer = 2, Reference = "Lucas 6:13" },

        // Mediana
        new Question { Id = 21, Text = "21 - Qual era o nome do irmão de Moisés que o ajudou diante de Faraó?", Options = new(){"A) - Arão", "B) - Josué", "C) - Calebe", "D) - Samuel"}, Answer = 0, Reference = "Êxodo 7:1-2" },
        new Question { Id = 22, Text = "22 - Quem foi escolhido para substituir Judas Iscariotes entre os doze apóstolos?", Options = new(){"A) - Barnabé", "B) - Matias", "C) - Silas", "D) - Estêvão"}, Answer = 1, Reference = "Atos 1:26" },
        new Question { Id = 23, Text = "23 - Em qual monte a arca de Noé repousou após o dilúvio?", Options = new(){"A) - Sinai", "B) - Carmelo", "C) - Ararate", "D) - Moriá"}, Answer = 2, Reference = "Gênesis 8:4" },
        new Question { Id = 24, Text = "24 - Quem foi a mulher que se tornou rainha após Ester?", Options = new(){"A) - Vasti", "B) - Débora", "C) - Rute", "D) - Ester"}, Answer = 3, Reference = "Ester 2:17" },
        new Question { Id = 25, Text = "25 - Qual profeta enfrentou os profetas de Baal no Monte Carmelo?", Options = new(){"A) - Eliseu", "B) - Isaías", "C) - Jeremias", "D) - Elias"}, Answer = 3, Reference = "1 Reis 18:19-39" },
        new Question { Id = 26, Text = "26 - Quem escreveu a maioria dos Salmos?", Options = new(){"A) - Salomão", "B) - Davi", "C) - Asafe", "D) - Moisés"}, Answer = 1, Reference = "Salmos 23:1 (título)" },
        new Question { Id = 27, Text = "27 - Qual discípulo era cobrador de impostos antes de seguir Jesus?", Options = new(){"A) - Mateus", "B) - Pedro", "C) - Filipe", "D) - Tomé"}, Answer = 0, Reference = "Mateus 9:9" },
        new Question { Id = 28, Text = "28 - Quem teve a visão dos ossos secos que voltaram à vida?", Options = new(){"A) - Daniel", "B) - Ezequiel", "C) - Isaías", "D) - Oséias"}, Answer = 1, Reference = "Ezequiel 37:1-10" },
        new Question { Id = 29, Text = "29 - Qual foi o primeiro milagre de Jesus segundo o Evangelho de João?", Options = new(){"A) - Multiplicação dos pães", "B) - Cura de um cego", "C) - Transformar água em vinho", "D) - Andar sobre o mar"}, Answer = 2, Reference = "João 2:1-11" },
        new Question { Id = 30, Text = "30 - Em qual cidade Paulo foi convertido após encontrar Jesus no caminho?", Options = new(){"A) - Jerusalém", "B) - Antioquia", "C) - Damasco", "D) - Éfeso"}, Answer = 2, Reference = "Atos 9:3-6" },

        // Mediana mais
        new Question { Id = 31, Text = "31 - Qual era o nome do pai de João Batista?", Options = new(){"A) - Zacarias", "B) - Simeão", "C) - Eli", "D) - Natã"}, Answer = 0, Reference = "Lucas 1:13" },
        new Question { Id = 32, Text = "32 - Quem tocou na arca da aliança e morreu imediatamente?", Options = new(){"A) - Uzá", "B) - Hofni", "C) - Fineias", "D) - Abinadabe"}, Answer = 0, Reference = "2 Samuel 6:6-7" },
        new Question { Id = 33, Text = "33 - Qual juiz fez um voto precipitado envolvendo sua filha?", Options = new(){"A) - Gideão", "B) - Sansão", "C) - Jefté", "D) - Débora"}, Answer = 2, Reference = "Juízes 11:30-35" },
        new Question { Id = 34, Text = "34 - Quem foi o rei que pediu sabedoria a Deus em vez de riquezas?", Options = new(){"A) - Davi", "B) - Salomão", "C) - Ezequias", "D) - Josias"}, Answer = 1, Reference = "1 Reis 3:9-12" },
        new Question { Id = 35, Text = "35 - Qual profeta foi alimentado por corvos no deserto?", Options = new(){"A) - Eliseu", "B) - Elias", "C) - Isaías", "D) - Amós"}, Answer = 1, Reference = "1 Reis 17:4-6" },
        new Question { Id = 36, Text = "36 - Quem escreveu o livro de Lamentações?", Options = new(){"A) - Isaías", "B) - Jeremias", "C) - Ezequiel", "D) - Daniel"}, Answer = 1, Reference = "Lamentações 1:1 (tradição)" },
        new Question { Id = 37, Text = "37 - Qual discípulo duvidou da ressurreição de Jesus até vê-lo?", Options = new(){"A) - Filipe", "B) - Tomé", "C) - Bartolomeu", "D) - André"}, Answer = 1, Reference = "João 20:27" },
        new Question { Id = 38, Text = "38 - Em qual ilha João recebeu a revelação do Apocalipse?", Options = new(){"A) - Creta", "B) - Malta", "C) - Patmos", "D) - Chipre"}, Answer = 2, Reference = "Apocalipse 1:9" },
        new Question { Id = 39, Text = "39 - Quem foi o general sírio curado de lepra ao mergulhar no Jordão?", Options = new(){"A) - Naamã", "B) - Senaqueribe", "C) - Hadade", "D) - Rezim"}, Answer = 0, Reference = "2 Reis 5:14" },
        new Question { Id = 40, Text = "40 - Qual era a profissão de Lucas, autor de um dos Evangelhos?", Options = new(){"A) - Pescador", "B) - Médico", "C) - Cobrador de impostos", "D) - Tendeiro"}, Answer = 1, Reference = "Colossenses 4:14" },

        new Question { Id = 41, Text = "41 - Quem teve seu nome mudado para Israel após lutar com o anjo?", Options = new(){"A) - Esaú", "B) - Jacó", "C) - José", "D) - Benjamim"}, Answer = 1, Reference = "Gênesis 32:28" },
        new Question { Id = 42, Text = "42 - Qual rei adoeceu e teve sua vida prolongada por mais quinze anos?", Options = new(){"A) - Manassés", "B) - Ezequias", "C) - Josafá", "D) - Roboão"}, Answer = 1, Reference = "2 Reis 20:5-6" },
        new Question { Id = 43, Text = "43 - Quem aconselhou Davi a não matar Saul na caverna?", Options = new(){"A) - Natã", "B) - Abiatar", "C) - Seus homens", "D) - Jônatas"}, Answer = 2, Reference = "1 Samuel 24:4" },
        new Question { Id = 44, Text = "44 - Qual livro da Bíblia não menciona diretamente o nome de Deus?", Options = new(){"A) - Ester", "B) - Cantares", "C) - Rute", "D) - Obadias"}, Answer = 0, Reference = "Ester (conteúdo geral)" },
        new Question { Id = 45, Text = "45 - Quem foi o discípulo que substituiu Judas Iscariotes?", Options = new(){"A) - Justo", "B) - Matias", "C) - Silas", "D) - Barsabás"}, Answer = 1, Reference = "Atos 1:26" },
        new Question { Id = 46, Text = "46 - Qual igreja recebeu as cartas do Apocalipse em maior número total de sete?", Options = new(){"A) - Igrejas da Galácia", "B) - Igrejas da Ásia", "C) - Igrejas da Judeia", "D) - Igrejas da Macedônia"}, Answer = 1, Reference = "Apocalipse 1:11" },
        new Question { Id = 47, Text = "47 - Quem caiu da janela enquanto Paulo pregava e morreu, sendo depois ressuscitado?", Options = new(){"A) - Êutico", "B) - Trófimo", "C) - Onésimo", "D) - Gaio"}, Answer = 0, Reference = "Atos 20:9-10" },
        new Question { Id = 48, Text = "48 - Qual era o nome hebraico de Daniel?", Options = new(){"A) - Beltessazar", "B) - Sadraque", "C) - Abede-Nego", "D) - Aspenaz"}, Answer = 0, Reference = "Daniel 1:7" },
        new Question { Id = 49, Text = "49 - Quem foi o sogro de Moisés?", Options = new(){"A) - Labão", "B) - Jetro", "C) - Balaão", "D) - Corá"}, Answer = 1, Reference = "Êxodo 3:1" },
        new Question { Id = 50, Text = "50 - Em qual cidade os discípulos foram chamados cristãos pela primeira vez?", Options = new(){"A) - Jerusalém", "B) - Antioquia", "C) - Roma", "D) - Éfeso"}, Answer = 1, Reference = "Atos 11:26" },

        // Dificil

        new Question { Id = 51, Text = "51 - Qual era o nome da esposa de Abraão que lhe deu Isaque?", Options = new(){"A) - Rebeca", "B) - Sara", "C) - Raquel", "D) - Lia"}, Answer = 1, Reference = "Gênesis 21:1-3" },
        new Question { Id = 52, Text = "52 - Quem matou Eglom, rei de Moabe, libertando Israel?", Options = new(){"A) - Gideão", "B) - Ehud", "C) - Baraque", "D) - Jefté"}, Answer = 1, Reference = "Juízes 3:20-21" },
        new Question { Id = 53, Text = "53 - Qual profeta anunciou o nascimento do Messias em Belém?", Options = new(){"A) - Isaías", "B) - Jeremias", "C) - Miqueias", "D) - Zacarias"}, Answer = 2, Reference = "Miqueias 5:2" },
        new Question { Id = 54, Text = "54 - Quem foi o escriba que liderou o retorno do povo e ensinou a Lei após o exílio?", Options = new(){"A) - Neemias", "B) - Esdras", "C) - Zorobabel", "D) - Ageu"}, Answer = 1, Reference = "Esdras 7:10" },
        new Question { Id = 55, Text = "55 - Qual rei de Judá encontrou o Livro da Lei durante reformas no templo?", Options = new(){"A) - Ezequias", "B) - Manassés", "C) - Josias", "D) - Acaz"}, Answer = 2, Reference = "2 Reis 22:8" },
        new Question { Id = 56, Text = "56 - Quem viu uma escada que chegava ao céu em sonho?", Options = new(){"A) - José", "B) - Moisés", "C) - Jacó", "D) - Samuel"}, Answer = 2, Reference = "Gênesis 28:12" },
        new Question { Id = 57, Text = "57 - Qual apóstolo foi morto à espada por ordem de Herodes?", Options = new(){"A) - Pedro", "B) - João", "C) - Tiago, irmão de João", "D) - André"}, Answer = 2, Reference = "Atos 12:1-2" },
        new Question { Id = 58, Text = "58 - Quem foi o profeta que se casou com uma mulher adúltera como sinal profético?", Options = new(){"A) - Oséias", "B) - Amós", "C) - Joel", "D) - Sofonias"}, Answer = 0, Reference = "Oséias 1:2-3" },
        new Question { Id = 59, Text = "59 - Qual discípulo era conhecido como 'filho da consolação'?", Options = new(){"A) - Silas", "B) - Barnabé", "C) - Marcos", "D) - Timóteo"}, Answer = 1, Reference = "Atos 4:36" },
        new Question { Id = 60, Text = "60 - Quem sucedeu Moisés na liderança de Israel?", Options = new(){"A) - Calebe", "B) - Arão", "C) - Josué", "D) - Samuel"}, Answer = 2, Reference = "Deuteronômio 34:9" },
        
        new Question { Id = 61, Text = "61 - Qual profeta confrontou Davi após o pecado com Bate-Seba?", Options = new(){"A) - Gade", "B) - Natã", "C) - Samuel", "D) - Elias"}, Answer = 1, Reference = "2 Samuel 12:1-7" },
        new Question { Id = 62, Text = "62 - Quem escreveu a maioria das cartas do Novo Testamento?", Options = new(){"A) - Pedro", "B) - João", "C) - Paulo", "D) - Tiago"}, Answer = 2, Reference = "Romanos 1:1" },
        new Question { Id = 63, Text = "63 - Qual era o nome do sumo sacerdote no julgamento de Jesus?", Options = new(){"A) - Anás", "B) - Caifás", "C) - Gamaliel", "D) - Eleazar"}, Answer = 1, Reference = "Mateus 26:57" },
        new Question { Id = 64, Text = "64 - Quem ajudou Jesus a carregar a cruz?", Options = new(){"A) - José de Arimateia", "B) - Simão Cireneu", "C) - Nicodemos", "D) - Barrabás"}, Answer = 1, Reference = "Lucas 23:26" },
        new Question { Id = 65, Text = "65 - Qual livro bíblico contém a descrição da armadura de Deus?", Options = new(){"A) - Romanos", "B) - Efésios", "C) - Hebreus", "D) - Colossenses"}, Answer = 1, Reference = "Efésios 6:11-17" },
        new Question { Id = 66, Text = "66 - Quem foi o rei babilônico que teve o sonho da grande estátua interpretado por Daniel?", Options = new(){"A) - Nabucodonosor", "B) - Belsazar", "C) - Dario", "D) - Ciro"}, Answer = 0, Reference = "Daniel 2:1" },
        new Question { Id = 67, Text = "67 - Qual mulher julgou Israel como profetisa?", Options = new(){"A) - Ana", "B) - Débora", "C) - Hulda", "D) - Abigail"}, Answer = 1, Reference = "Juízes 4:4" },
        new Question { Id = 68, Text = "68 - Quem foi lançado no mar e depois salvo por um grande peixe?", Options = new(){"A) - Elias", "B) - Jonas", "C) - Paulo", "D) - Pedro"}, Answer = 1, Reference = "Jonas 1:15-17" },
        new Question { Id = 69, Text = "69 - Qual era o nome do pai do rei Davi?", Options = new(){"A) - Obede", "B) - Jessé", "C) - Eliabe", "D) - Samuel"}, Answer = 1, Reference = "1 Samuel 16:1" },
        new Question { Id = 70, Text = "70 - Em qual livro está a visão do vale de ossos secos?", Options = new(){"A) - Isaías", "B) - Jeremias", "C) - Ezequiel", "D) - Daniel"}, Answer = 2, Reference = "Ezequiel 37:1-10" },

        // Teologico
        new Question { Id = 71, Text = "71 - Qual termo hebraico é usado em Gênesis 1 para 'criar' no sentido exclusivo da ação divina?", Options = new(){"A) - Asah", "B) - Yatsar", "C) - Bara", "D) - Banah"}, Answer = 2, Reference = "Gênesis 1:1" },
        new Question { Id = 72, Text = "72 - Qual é o nome do pacto teológico estabelecido por Deus com Abraão envolvendo descendência e terra?", Options = new(){"A) - Pacto Sinaítico", "B) - Pacto Davídico", "C) - Pacto Abraâmico", "D) - Novo Pacto"}, Answer = 2, Reference = "Gênesis 15:18" },
        new Question { Id = 73, Text = "73 - Qual profeta introduz fortemente o conceito do 'Servo Sofredor'?", Options = new(){"A) - Jeremias", "B) - Isaías", "C) - Ezequiel", "D) - Daniel"}, Answer = 1, Reference = "Isaías 53:3-7" },
        new Question { Id = 74, Text = "74 - Em qual língua foi majoritariamente escrito o Antigo Testamento?", Options = new(){"A) - Grego koiné", "B) - Latim", "C) - Hebraico", "D) - Aramaico"}, Answer = 2, Reference = "Contexto textual do AT" },
        new Question { Id = 75, Text = "75 - Qual concílio da igreja primitiva tratou diretamente da questão da circuncisão dos gentios?", Options = new(){"A) - Concílio de Niceia", "B) - Concílio de Jerusalém", "C) - Concílio de Calcedônia", "D) - Concílio de Éfeso"}, Answer = 1, Reference = "Atos 15:6-29" },
        new Question { Id = 76, Text = "76 - Qual evangelho apresenta Jesus principalmente como o Verbo eterno?", Options = new(){"A) - Mateus", "B) - Marcos", "C) - Lucas", "D) - João"}, Answer = 3, Reference = "João 1:1" },
        new Question { Id = 77, Text = "77 - Qual doutrina é central em Romanos 5 ao tratar da justificação?", Options = new(){"A) - Justificação pelas obras", "B) - Justificação pela fé", "C) - Predestinação nacional", "D) - Santificação progressiva"}, Answer = 1, Reference = "Romanos 5:1" },
        new Question { Id = 78, Text = "78 - Qual figura do Antigo Testamento é explicitamente chamada de tipo de Cristo em Hebreus?", Options = new(){"A) - Moisés", "B) - Arão", "C) - Melquisedeque", "D) - Josué"}, Answer = 2, Reference = "Hebreus 7:3" },
        new Question { Id = 79, Text = "79 - Qual termo grego descreve o esvaziamento voluntário de Cristo em Filipenses 2?", Options = new(){"A) - Dikaiosyne", "B) - Kenosis", "C) - Parousia", "D) - Koinonia"}, Answer = 1, Reference = "Filipenses 2:7" },
        new Question { Id = 80, Text = "80 - Qual gênero literário predomina no livro de Apocalipse?", Options = new(){"A) - Narrativa histórica", "B) - Poesia sapiencial", "C) - Literatura apocalíptica", "D) - Epístola pastoral"}, Answer = 2, Reference = "Apocalipse 1:1" },
        
        new Question { Id = 81, Text = "81 - Qual profeta do pós-exílio enfatiza a reconstrução do templo junto com Zorobabel?", Options = new(){"A) - Ageu", "B) - Amós", "C) - Miqueias", "D) - Naum"}, Answer = 0, Reference = "Ageu 1:1" },
        new Question { Id = 82, Text = "82 - Qual expressão latina resume a doutrina reformada da salvação somente pela graça?", Options = new(){"A) - Sola Scriptura", "B) - Solus Christus", "C) - Sola Gratia", "D) - Soli Deo Gloria"}, Answer = 2, Reference = "Efésios 2:8" },
        new Question { Id = 83, Text = "83 - Qual livro apresenta a teologia do sumo sacerdócio celestial de Cristo de forma sistemática?", Options = new(){"A) - Romanos", "B) - Hebreus", "C) - Levítico", "D) - Colossenses"}, Answer = 1, Reference = "Hebreus 4:14-16" },
        new Question { Id = 84, Text = "84 - Qual é o termo teológico para o estudo das últimas coisas?", Options = new(){"A) - Soteriologia", "B) - Pneumatologia", "C) - Escatologia", "D) - Cristologia"}, Answer = 2, Reference = "Apocalipse 22:12" },
        new Question { Id = 85, Text = "85 - Qual personagem do Antigo Testamento é citado em Romanos 4 como exemplo de justificação pela fé?", Options = new(){"A) - Moisés", "B) - Davi", "C) - Abraão", "D) - Elias"}, Answer = 2, Reference = "Romanos 4:3" },
        new Question { Id = 86, Text = "86 - Qual evangelho possui maior ênfase na ação imediata de Jesus, usando frequentemente 'logo'?", Options = new(){"A) - Mateus", "B) - Marcos", "C) - Lucas", "D) - João"}, Answer = 1, Reference = "Marcos 1:10" },
        new Question { Id = 87, Text = "87 - Qual livro sapiencial discute profundamente o problema do sofrimento do justo?", Options = new(){"A) - Provérbios", "B) - Eclesiastes", "C) - Jó", "D) - Cantares"}, Answer = 2, Reference = "Jó 1:1" },
        new Question { Id = 88, Text = "88 - Qual apóstolo é tradicionalmente associado à autoria do Apocalipse?", Options = new(){"A) - Pedro", "B) - Paulo", "C) - João", "D) - Tiago"}, Answer = 2, Reference = "Apocalipse 1:1-2" },
        new Question { Id = 89, Text = "89 - Qual conceito teológico descreve Deus tornando-se humano em Cristo?", Options = new(){"A) - Exaltação", "B) - Encarnação", "C) - Propiciação", "D) - Glorificação"}, Answer = 1, Reference = "João 1:14" },
        new Question { Id = 90, Text = "90 - Qual é o termo para a revelação progressiva de Deus ao longo da história bíblica?", Options = new(){"A) - Inspiração verbal", "B) - Revelação progressiva", "C) - Iluminação espiritual", "D) - Tradição apostólica"}, Answer = 1, Reference = "Hebreus 1:1-2" },


        };

        private static int Current = 0;
        private static int ScoreBlue = 0;
        private static string TeamTurn = "blue";

        public IActionResult Index()
        {
            var index = HttpContext.Session.GetInt32("index") ?? 0;
            Current = index;

            ViewBag.ScoreBlue = ScoreBlue;
            ViewBag.TeamTurn = TeamTurn;

            return View(Questions[Current]);
        }

        public IActionResult GoTo(int number)
        {
            if (number < 1 || number > Questions.Count)
                number = 1;

            HttpContext.Session.SetInt32("index", number - 1);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Answer(int option)
        {
            var question = Questions[Current];

            if (option == question.Answer)
            {
                if (TeamTurn == "blue") ScoreBlue++;
            }

            return RedirectToAction("Result");
        }

        public IActionResult Result()
        {
            ViewBag.ScoreBlue = ScoreBlue;
            ViewBag.Reference = Questions[Current].Reference;

            var correctDescription = Questions[Current].Options[Questions[Current].Answer];

            ViewBag.resposta = correctDescription;

            return View(Questions[Current]);
        }


        public IActionResult Next()
        {
            if (Current + 1 < Questions.Count)
            {
                Current++;
                TeamTurn = TeamTurn == "blue" ? "red" : "blue";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Restart()
        {
            Current = 0;
            ScoreBlue = 0;
            TeamTurn = "blue";
            return RedirectToAction("Index");
        }
    }
}